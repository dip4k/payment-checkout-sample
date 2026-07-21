import type { CatalogueItem } from '../../shared/api/checkoutApi';
import './catalogue-panel.css';

type OrderLineState = {
  productId: string;
  quantityInput: string;
};

type CataloguePanelProps = {
  items: CatalogueItem[];
  loading: boolean;
  error: string;
  quantities: OrderLineState[];
  onQuantityChange: (productId: string, quantityInput: string) => void;
};

export function CataloguePanel({
  items,
  loading,
  error,
  quantities,
  onQuantityChange,
}: CataloguePanelProps) {
  return (
    <section className="panel catalogue-panel">
      <div className="panel-header">
        <div>
          <p className="section-tag">Catalogue</p>
          <h2>Build the order</h2>
        </div>
        <p className="section-copy">Choose quantities directly against the seeded backend products.</p>
      </div>

      {loading ? <p className="panel-message">Loading catalogue...</p> : null}
      {error ? <p className="panel-message panel-error">{error}</p> : null}

      {!loading && !error ? (
        <div className="catalogue-grid">
          {items.map((item) => {
            const quantityInput = quantities.find((line) => line.productId === item.id)?.quantityInput ?? '0';

            return (
              <article key={item.id} className="catalogue-card">
                <div>
                  <p className="catalogue-price">${item.unitPrice.toFixed(2)}</p>
                  <h3>{item.name}</h3>
                  <p className="catalogue-meta">
                    {item.isTaxable ? 'Taxable at standard rate' : 'Tax exempt'}
                  </p>
                </div>

                <div className="catalogue-footer">
                  <span className="catalogue-version">v{item.version}</span>
                  <label>
                    <span>Qty</span>
                    <input
                      type="text"
                      inputMode="numeric"
                      pattern="[0-9]*"
                      value={quantityInput}
                      onChange={(event) => onQuantityChange(item.id, event.target.value)}
                    />
                  </label>
                </div>
              </article>
            );
          })}
        </div>
      ) : null}
    </section>
  );
}