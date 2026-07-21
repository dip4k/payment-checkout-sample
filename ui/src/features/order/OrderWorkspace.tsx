import type { CalculateOrderResponse, DiscountType } from '../../shared/api/checkoutApi';
import './order-workspace.css';

type DiscountState = {
  type: DiscountType;
  value: string;
};

export type SubmittedOrderSummary = {
  submittedAtIso: string;
  lines: Array<{
    productId: string;
    productName: string;
    quantity: number;
    unitPrice: number;
    lineTotal: number;
  }>;
  subtotal: number;
  discountApplied: number;
  tax: number;
  total: number;
};

type OrderWorkspaceProps = {
  discount: DiscountState;
  calculation: CalculateOrderResponse | null;
  isDirty: boolean;
  isCalculating: boolean;
  isSubmitting: boolean;
  submittedOrder: SubmittedOrderSummary | null;
  error: string;
  submissionMessage: string;
  onDiscountChange: (discount: DiscountState) => void;
  onSubmit: () => Promise<void>;
};

export function OrderWorkspace({
  discount,
  calculation,
  isDirty,
  isCalculating,
  isSubmitting,
  submittedOrder,
  error,
  submissionMessage,
  onDiscountChange,
  onSubmit,
}: OrderWorkspaceProps) {
  return (
    <section className="panel order-panel">
      <div className="panel-header">
        <div>
          <p className="section-tag">Order</p>
          <h2>Discount and totals</h2>
        </div>
        <p className="section-copy">Totals stay visible while you calculate, inspect, and submit.</p>
      </div>

      <div className="order-form-grid">
        <label>
          <span>Discount type</span>
          <select
            value={discount.type}
            onChange={(event) =>
              onDiscountChange({
                ...discount,
                type: event.target.value as DiscountType,
                value: event.target.value === 'None' ? '' : discount.value,
              })
            }
          >
            <option value="None">None</option>
            <option value="Percentage">Percentage</option>
            <option value="FixedAmount">Fixed amount</option>
          </select>
        </label>

        <label>
          <span>Discount value</span>
          <input
            type="number"
            min="0"
            step="0.01"
            value={discount.value}
            disabled={discount.type === 'None'}
            placeholder={discount.type === 'Percentage' ? '10' : '5.00'}
            onChange={(event) => onDiscountChange({ ...discount, value: event.target.value })}
          />
        </label>
      </div>

      <div className="totals-card">
        <div className="totals-row">
          <span>Subtotal</span>
          <strong>{formatMoney(calculation?.subtotal)}</strong>
        </div>
        <div className="totals-row">
          <span>Discount</span>
          <strong>{formatMoney(calculation?.discountApplied)}</strong>
        </div>
        <div className="totals-row">
          <span>Tax</span>
          <strong>{formatMoney(calculation?.tax)}</strong>
        </div>
        <div className="totals-row totals-total">
          <span>Total</span>
          <strong>{formatMoney(calculation?.total)}</strong>
        </div>
      </div>

      <div className="split-card">
        <div className="split-header">
          <span>Part Two split</span>
          <strong>3-way payment breakdown</strong>
        </div>
        {calculation?.splitShares?.length ? (
          <div className="split-grid">
            {calculation.splitShares.map((share) => (
              <div key={share.payer} className="split-share">
                <span>Payer {share.payer}</span>
                <strong>{formatMoney(share.amount)}</strong>
              </div>
            ))}
          </div>
        ) : (
          <p className="split-empty">Calculate or submit an order to preview the three-way payment split.</p>
        )}
      </div>

      <div className="status-stack">
        {isDirty ? <p className="status-chip status-warn">Inputs changed. Totals will refresh automatically.</p> : null}
        {error ? <p className="status-chip status-error">{error}</p> : null}
        {submissionMessage ? <p className="status-chip status-success">{submissionMessage}</p> : null}
      </div>

      <div className="button-row">
        <button className="primary-button" type="button" onClick={() => void onSubmit()} disabled={isSubmitting || isCalculating}>
          {isSubmitting ? 'Submitting...' : 'Submit order'}
        </button>
      </div>

      {submittedOrder ? (
        <div className="submitted-order-card">
          <div className="submitted-order-header">
            <span>Submitted order</span>
            <strong>{formatDateTime(submittedOrder.submittedAtIso)}</strong>
          </div>

          <div className="submitted-order-lines">
            {submittedOrder.lines.map((line) => (
              <div key={line.productId} className="submitted-order-line">
                <div>
                  <p>{line.productName}</p>
                  <span>
                    {line.quantity} x {formatMoney(line.unitPrice)}
                  </span>
                </div>
                <strong>{formatMoney(line.lineTotal)}</strong>
              </div>
            ))}
          </div>

          <div className="submitted-order-totals">
            <div className="submitted-order-row">
              <span>Subtotal</span>
              <strong>{formatMoney(submittedOrder.subtotal)}</strong>
            </div>
            <div className="submitted-order-row">
              <span>Discount</span>
              <strong>{formatMoney(submittedOrder.discountApplied)}</strong>
            </div>
            <div className="submitted-order-row">
              <span>Tax</span>
              <strong>{formatMoney(submittedOrder.tax)}</strong>
            </div>
            <div className="submitted-order-row submitted-order-total">
              <span>Total</span>
              <strong>{formatMoney(submittedOrder.total)}</strong>
            </div>
          </div>
        </div>
      ) : null}
    </section>
  );
}

function formatMoney(value?: number) {
  return new Intl.NumberFormat('en-GB', {
    style: 'currency',
    currency: 'GBP',
  }).format(value ?? 0);
}

function formatDateTime(value: string) {
  return new Intl.DateTimeFormat('en-GB', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value));
}