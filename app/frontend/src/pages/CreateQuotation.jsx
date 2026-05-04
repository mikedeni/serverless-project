import React, { useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import api from '../utils/api';

const CreateQuotation = () => {
    const { projectId } = useParams();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const [formData, setFormData] = useState({
        projectId: parseInt(projectId),
        clientName: '',
        clientAddress: '',
        clientPhone: '',
        markupPercent: 0,
        discount: 0,
        taxPercent: 7,
        note: '',
        validUntil: ''
    });

    const [items, setItems] = useState([
        { description: '', qty: 1, unit: 'ชิ้น', unitPrice: 0 }
    ]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: ['markupPercent', 'discount', 'taxPercent'].includes(name) ? parseFloat(value) || 0 : value
        }));
    };

    const handleItemChange = (index, field, value) => {
        const updated = [...items];
        updated[index] = {
            ...updated[index],
            [field]: ['qty', 'unitPrice'].includes(field) ? parseFloat(value) || 0 : value
        };
        setItems(updated);
    };

    const addItem = () => {
        setItems([...items, { description: '', qty: 1, unit: 'ชิ้น', unitPrice: 0 }]);
    };

    const removeItem = (index) => {
        if (items.length <= 1) return;
        setItems(items.filter((_, i) => i !== index));
    };

    // Calculations
    const subTotal = items.reduce((sum, item) => sum + (item.qty * item.unitPrice), 0);
    const markupAmount = subTotal * (formData.markupPercent / 100);
    const afterMarkup = subTotal + markupAmount;
    const afterDiscount = afterMarkup - formData.discount;
    const taxAmount = afterDiscount * (formData.taxPercent / 100);
    const grandTotal = afterDiscount + taxAmount;

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const payload = {
                ...formData,
                validUntil: formData.validUntil || null,
                items: items.filter(i => i.description.trim() !== '')
            };
            await api.post('/quotations', payload);
            navigate(`/projects/${projectId}/quotations`);
        } catch (err) {
            setError(err.message || 'Failed to create quotation');
            setLoading(false);
        }
    };

    const inputStyle = {
        width: '100%', padding: '12px', background: 'rgba(0,0,0,0.2)',
        border: '1px solid rgba(255,255,255,0.1)', borderRadius: '8px',
        color: '#fff', fontSize: '14px', outline: 'none'
    };

    const labelStyle = { display: 'block', marginBottom: '6px', fontSize: '12px', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.5px' };

    return (
        <div className="page-container fadeIn">
            <header className="top-header">
                <div>
                    <Link to={`/projects/${projectId}/quotations`} className="breadcrumb">← Back to Quotations</Link>
                    <h1>Create Quotation / BOQ</h1>
                </div>
            </header>

            <form onSubmit={handleSubmit}>
                {error && <div className="auth-error">{error}</div>}

                {/* Client Info */}
                <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px' }}>
                    <h3 style={{ marginBottom: '20px', fontSize: '16px' }}>Client Information</h3>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                        <div>
                            <label style={labelStyle}>Client Name *</label>
                            <input style={inputStyle} name="clientName" required value={formData.clientName} onChange={handleChange} placeholder="ชื่อลูกค้า" />
                        </div>
                        <div>
                            <label style={labelStyle}>Phone</label>
                            <input style={inputStyle} name="clientPhone" value={formData.clientPhone} onChange={handleChange} placeholder="เบอร์โทร" />
                        </div>
                        <div style={{ gridColumn: 'span 2' }}>
                            <label style={labelStyle}>Address</label>
                            <input style={inputStyle} name="clientAddress" value={formData.clientAddress} onChange={handleChange} placeholder="ที่อยู่" />
                        </div>
                        <div>
                            <label style={labelStyle}>Valid Until</label>
                            <input style={inputStyle} type="date" name="validUntil" value={formData.validUntil} onChange={handleChange} />
                        </div>
                        <div>
                            <label style={labelStyle}>Note</label>
                            <input style={inputStyle} name="note" value={formData.note} onChange={handleChange} placeholder="หมายเหตุ" />
                        </div>
                    </div>
                </div>

                {/* BOQ Items */}
                <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                        <h3 style={{ fontSize: '16px' }}>BOQ Items</h3>
                        <button type="button" onClick={addItem} className="btn-secondary" style={{ fontSize: '13px' }}>+ Add Item</button>
                    </div>

                    <div className="table-wrapper">
                        <table className="modern-table">
                            <thead>
                                <tr>
                                    <th style={{ width: '40px' }}>#</th>
                                    <th>Description</th>
                                    <th style={{ width: '80px' }}>Qty</th>
                                    <th style={{ width: '80px' }}>Unit</th>
                                    <th style={{ width: '120px' }}>Unit Price</th>
                                    <th style={{ width: '120px' }}>Amount</th>
                                    <th style={{ width: '40px' }}></th>
                                </tr>
                            </thead>
                            <tbody>
                                {items.map((item, idx) => (
                                    <tr key={idx}>
                                        <td style={{ color: 'var(--text-muted)' }}>{idx + 1}</td>
                                        <td>
                                            <input style={{ ...inputStyle, padding: '8px' }} value={item.description} onChange={(e) => handleItemChange(idx, 'description', e.target.value)} placeholder="รายละเอียดงาน" required />
                                        </td>
                                        <td>
                                            <input style={{ ...inputStyle, padding: '8px' }} type="number" min="0" step="0.01" value={item.qty} onChange={(e) => handleItemChange(idx, 'qty', e.target.value)} />
                                        </td>
                                        <td>
                                            <input style={{ ...inputStyle, padding: '8px' }} value={item.unit} onChange={(e) => handleItemChange(idx, 'unit', e.target.value)} />
                                        </td>
                                        <td>
                                            <input style={{ ...inputStyle, padding: '8px' }} type="number" min="0" step="0.01" value={item.unitPrice} onChange={(e) => handleItemChange(idx, 'unitPrice', e.target.value)} />
                                        </td>
                                        <td style={{ fontWeight: 600, color: '#34D399' }}>
                                            ฿{(item.qty * item.unitPrice).toLocaleString()}
                                        </td>
                                        <td>
                                            {items.length > 1 && (
                                                <button type="button" onClick={() => removeItem(idx)} style={{ background: 'none', border: 'none', color: '#FCA5A5', cursor: 'pointer', fontSize: '16px' }}>✕</button>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* Pricing */}
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginBottom: '24px' }}>
                    <div className="glass-panel" style={{ padding: '24px' }}>
                        <h3 style={{ marginBottom: '20px', fontSize: '16px' }}>Pricing Options</h3>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                            <div>
                                <label style={labelStyle}>Markup %</label>
                                <input style={inputStyle} type="number" name="markupPercent" min="0" step="0.01" value={formData.markupPercent} onChange={handleChange} />
                            </div>
                            <div>
                                <label style={labelStyle}>Discount (฿)</label>
                                <input style={inputStyle} type="number" name="discount" min="0" step="0.01" value={formData.discount} onChange={handleChange} />
                            </div>
                            <div>
                                <label style={labelStyle}>Tax %</label>
                                <input style={inputStyle} type="number" name="taxPercent" min="0" step="0.01" value={formData.taxPercent} onChange={handleChange} />
                            </div>
                        </div>
                    </div>
                    <div className="glass-panel summary-card">
                        <h3 style={{ marginBottom: '20px', fontSize: '16px' }}>Summary</h3>
                        <div className="summary-row"><span>SubTotal</span><span>฿{subTotal.toLocaleString()}</span></div>
                        {formData.markupPercent > 0 && <div className="summary-row"><span>Markup ({formData.markupPercent}%)</span><span className="text-success">+฿{markupAmount.toLocaleString()}</span></div>}
                        {formData.discount > 0 && <div className="summary-row"><span>Discount</span><span className="text-danger">-฿{formData.discount.toLocaleString()}</span></div>}
                        <div className="summary-row"><span>Tax ({formData.taxPercent}%)</span><span>฿{taxAmount.toLocaleString()}</span></div>
                        <hr className="divider" />
                        <div className="summary-row large">
                            <span>Grand Total</span>
                            <span className="highlight">฿{grandTotal.toLocaleString()}</span>
                        </div>
                    </div>
                </div>

                <button type="submit" className="btn-primary" disabled={loading} style={{ maxWidth: '300px' }}>
                    {loading ? <div className="spinner-small"></div> : 'Create Quotation'}
                </button>
            </form>
        </div>
    );
};

export default CreateQuotation;
