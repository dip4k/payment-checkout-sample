const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5152/api/v1';

export type DiscountType = 'None' | 'Percentage' | 'FixedAmount';

export type CatalogueItem = {
  id: string;
  name: string;
  unitPrice: number;
  isTaxable: boolean;
  version: number;
};

export type OrderRequest = {
  lineItems: Array<{
    productId: string;
    quantity: number;
    productVersion: number;
  }>;
  discount: {
    type: Exclude<DiscountType, 'None'>;
    value: number;
  } | null;
};

export type CalculateOrderResponse = {
  subtotal: number;
  discountApplied: number;
  tax: number;
  total: number;
  splitShares: Array<{
    payer: number;
    amount: number;
  }>;
};

type JsonError = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

export async function getCatalogue() {
  const response = await fetch(`${apiBaseUrl}/catalogue`);
  return readJson<CatalogueItem[]>(response);
}

export async function calculateOrder(request: OrderRequest, correlationId: string) {
  const response = await fetch(`${apiBaseUrl}/orders/calculate`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'X-Correlation-ID': correlationId,
    },
    body: JSON.stringify(request),
  });

  return readJson<CalculateOrderResponse>(response);
}

export async function submitOrder(
  request: OrderRequest,
  options: { idempotencyKey: string; correlationId: string },
) {
  const response = await fetch(`${apiBaseUrl}/orders/submit`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Idempotency-Key': options.idempotencyKey,
      'X-Correlation-ID': options.correlationId,
    },
    body: JSON.stringify(request),
  });

  return readJson<CalculateOrderResponse>(response);
}

async function readJson<T>(response: Response): Promise<T> {
  if (response.ok) {
    return (await response.json()) as T;
  }

  let message = `${response.status} ${response.statusText}`;

  try {
    const error = (await response.json()) as JsonError;
    if (error.errors) {
      const detail = Object.values(error.errors)
        .flat()
        .join(' ');

      if (detail) {
        message = detail;
      }
    } else if (error.detail || error.title) {
      message = [error.title, error.detail].filter(Boolean).join(' ');
    }
  } catch {
    message = `${message}. Request could not be completed.`;
  }

  throw new Error(message);
}