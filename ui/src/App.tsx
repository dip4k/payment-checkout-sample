import { useEffect, useState } from 'react';
import { CataloguePanel } from './features/catalogue/CataloguePanel';
import { OrderWorkspace, type SubmittedOrderSummary } from './features/order/OrderWorkspace';
import {
  calculateOrder,
  getCatalogue,
  submitOrder,
  type CalculateOrderResponse,
  type CatalogueItem,
  type DiscountType,
  type OrderRequest,
} from './shared/api/checkoutApi';
import './app.css';

type OrderLineState = {
  productId: string;
  quantityInput: string;
};

type DiscountState = {
  type: DiscountType;
  value: string;
};

const initialDiscount: DiscountState = {
  type: 'None',
  value: '',
};

function App() {
  const [catalogue, setCatalogue] = useState<CatalogueItem[]>([]);
  const [catalogueError, setCatalogueError] = useState<string>('');
  const [catalogueLoading, setCatalogueLoading] = useState(true);
  const [orderLines, setOrderLines] = useState<OrderLineState[]>([]);
  const [discount, setDiscount] = useState<DiscountState>(initialDiscount);
  const [calculation, setCalculation] = useState<CalculateOrderResponse | null>(null);
  const [isDirty, setIsDirty] = useState(false);
  const [isCalculating, setIsCalculating] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formError, setFormError] = useState<string>('');
  const [submissionMessage, setSubmissionMessage] = useState<string>('');
  const [submittedOrder, setSubmittedOrder] = useState<SubmittedOrderSummary | null>(null);
  const [idempotencyKey, setIdempotencyKey] = useState<string>(() => crypto.randomUUID());
  const [correlationId, setCorrelationId] = useState<string>(() => crypto.randomUUID());

  useEffect(() => {
    let isMounted = true;

    async function loadCatalogue() {
      try {
        const response = await getCatalogue();
        if (!isMounted) {
          return;
        }

        setCatalogue(response);
        setOrderLines(response.map((item) => ({ productId: item.id, quantityInput: '0' })));
      } catch (error) {
        if (isMounted) {
          setCatalogueError(error instanceof Error ? error.message : 'Failed to load catalogue.');
        }
      } finally {
        if (isMounted) {
          setCatalogueLoading(false);
        }
      }
    }

    void loadCatalogue();

    return () => {
      isMounted = false;
    };
  }, []);

  function markDirty() {
    setIsDirty(true);
    setFormError('');
    setSubmissionMessage('');
  }

  function updateQuantity(productId: string, quantityInput: string) {
    const sanitizedQuantity = quantityInput === '' ? '' : quantityInput.replace(/[^0-9]/g, '');

    setOrderLines((current) =>
      current.map((line) => (line.productId === productId ? { ...line, quantityInput: sanitizedQuantity } : line)),
    );
    markDirty();
  }

  function updateDiscount(nextDiscount: DiscountState) {
    setDiscount(nextDiscount);
    markDirty();
  }

  function buildRequest(): OrderRequest {
    const selectedLines = orderLines
      .map((line) => {
        const parsedQuantity = parseQuantity(line.quantityInput);

        if (Number.isNaN(parsedQuantity) || parsedQuantity < 0 || !Number.isInteger(parsedQuantity)) {
          throw new Error('Quantity must be a whole number greater than or equal to zero.');
        }

        if (parsedQuantity === 0) {
          return null;
        }

        const product = catalogue.find((item) => item.id === line.productId);

        if (!product) {
          throw new Error('Selected product was not found in the catalogue.');
        }

        return {
          productId: product.id,
          quantity: parsedQuantity,
          productVersion: product.version,
        };
      })
      .filter((line): line is Exclude<typeof line, null> => line !== null);

    if (selectedLines.length === 0) {
      throw new Error('Select at least one product quantity above zero.');
    }

    const trimmedValue = discount.value.trim();
    const parsedValue = trimmedValue === '' ? 0 : Number(trimmedValue);

    if (Number.isNaN(parsedValue) || parsedValue < 0) {
      throw new Error('Discount value must be a non-negative number.');
    }

    return {
      lineItems: selectedLines,
      discount:
        discount.type === 'None'
          ? null
          : {
              type: discount.type,
              value: parsedValue,
            },
    };
  }

  function buildRequestForAutoCalculate(): OrderRequest | null {
    const selectedLines = orderLines
      .map((line) => {
        const parsedQuantity = parseQuantity(line.quantityInput);

        if (Number.isNaN(parsedQuantity) || parsedQuantity < 0 || !Number.isInteger(parsedQuantity)) {
          return null;
        }

        if (parsedQuantity === 0) {
          return undefined;
        }

        const product = catalogue.find((item) => item.id === line.productId);

        if (!product) {
          return null;
        }

        return {
          productId: product.id,
          quantity: parsedQuantity,
          productVersion: product.version,
        };
      });

    if (selectedLines.some((line) => line === null)) {
      return null;
    }

    const lineItems = selectedLines.filter((line): line is OrderRequest['lineItems'][number] => line !== undefined);

    if (lineItems.length === 0) {
      return null;
    }

    if (discount.type === 'None') {
      return {
        lineItems,
        discount: null,
      };
    }

    const trimmedValue = discount.value.trim();
    if (trimmedValue === '') {
      return null;
    }

    const parsedDiscount = Number(trimmedValue);
    if (Number.isNaN(parsedDiscount) || parsedDiscount < 0) {
      return null;
    }

    return {
      lineItems,
      discount: {
        type: discount.type,
        value: parsedDiscount,
      },
    };
  }

  useEffect(() => {
    if (!isDirty || catalogueLoading || isSubmitting) {
      return;
    }

    const request = buildRequestForAutoCalculate();
    if (!request) {
      return;
    }

    const timeoutId = window.setTimeout(async () => {
      setIsCalculating(true);
      setFormError('');
      setSubmissionMessage('');

      try {
        const result = await calculateOrder(request, correlationId);
        setCalculation(result);
        setIsDirty(false);
      } catch (error) {
        setFormError(error instanceof Error ? error.message : 'Calculation failed.');
      } finally {
        setIsCalculating(false);
      }
    }, 300);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [isDirty, catalogueLoading, isSubmitting, orderLines, discount, correlationId]);

  async function handleSubmit() {
    setIsSubmitting(true);
    setFormError('');
    setSubmissionMessage('');

    try {
      const request = buildRequest();
      const result = await submitOrder(request, {
        idempotencyKey,
        correlationId,
      });

      setCalculation(result);
      setIsDirty(false);
      setSubmittedOrder(buildSubmittedOrderSummary(request, result, catalogue));
      setSubmissionMessage('Order submitted successfully. Reuse the same idempotency key to replay this result.');
      setIdempotencyKey(crypto.randomUUID());
      setCorrelationId(crypto.randomUUID());
    } catch (error) {
      setFormError(error instanceof Error ? error.message : 'Order submission failed.');
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="app-shell">
      <header className="hero-panel">
        <div>
          <p className="eyebrow">Phase 4</p>
          <h1>Checkout Console</h1>
          <p className="hero-copy">
            Operator-facing UI for catalogue selection, order calculation, and idempotent submission.
          </p>
        </div>
        <div className="hero-meta">
          <div>
            <span>API base</span>
            <strong>{import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5152/api/v1'}</strong>
          </div>
          <div>
            <span>Status</span>
            <strong>{catalogueLoading ? 'Loading catalogue' : 'Ready for checkout'}</strong>
          </div>
        </div>
      </header>

      <main className="workspace-grid">
        <CataloguePanel
          items={catalogue}
          loading={catalogueLoading}
          error={catalogueError}
          quantities={orderLines}
          onQuantityChange={updateQuantity}
        />
        <OrderWorkspace
          discount={discount}
          calculation={calculation}
          isDirty={isDirty}
          isCalculating={isCalculating}
          isSubmitting={isSubmitting}
          submittedOrder={submittedOrder}
          error={formError}
          submissionMessage={submissionMessage}
          onDiscountChange={updateDiscount}
          onSubmit={handleSubmit}
        />
      </main>
    </div>
  );
}

export default App;

function parseQuantity(quantityInput: string): number {
  const trimmed = quantityInput.trim();
  if (trimmed === '') {
    return 0;
  }

  return Number(trimmed);
}

function buildSubmittedOrderSummary(
  request: OrderRequest,
  result: CalculateOrderResponse,
  catalogue: CatalogueItem[],
): SubmittedOrderSummary {
  const lines = request.lineItems.map((line) => {
    const product = catalogue.find((item) => item.id === line.productId);
    const productName = product?.name ?? line.productId;
    const unitPrice = product?.unitPrice ?? 0;

    return {
      productId: line.productId,
      productName,
      quantity: line.quantity,
      unitPrice,
      lineTotal: unitPrice * line.quantity,
    };
  });

  return {
    submittedAtIso: new Date().toISOString(),
    lines,
    subtotal: result.subtotal,
    discountApplied: result.discountApplied,
    tax: result.tax,
    total: result.total,
  };
}