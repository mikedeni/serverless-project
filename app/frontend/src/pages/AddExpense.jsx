import React, { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import api from '../utils/api';
import { 
    Receipt, 
    ArrowLeft, 
    Check, 
    Calendar, 
    Tag, 
    DollarSign, 
    FileText 
} from 'lucide-react';

const AddExpense = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({ amount: '', category: 'material_cost', date: new Date().toISOString().split('T')[0], note: '' });
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        try {
            await api.post('/expenses', {
               projectId: parseInt(id),
               amount: parseFloat(formData.amount),
               category: formData.category,
               date: formData.date,
               note: formData.note
            });
            navigate(`/projects/${id}`);
        } catch (e) {
            alert(e.message);
            setLoading(false);
        }
    };

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
            <div className="glass-panel fadeIn" style={{ width: '100%', maxWidth: '500px', padding: '40px' }}>
                <Link to={`/projects/${id}`} className="btn-secondary" style={{ width: 'auto', marginBottom: '32px', textDecoration: 'none', border: 'none', background: 'none', padding: 0, color: 'var(--text-muted)' }}>
                    <ArrowLeft size={18} /> Back to Project
                </Link>
                
                <div style={{ display: 'flex', alignItems: 'center', gap: '16px', marginBottom: '32px' }}>
                    <div style={{ background: 'rgba(186, 26, 26, 0.1)', color: 'var(--error)', padding: '12px', borderRadius: '12px' }}>
                        <Receipt size={28} />
                    </div>
                    <h2 className="text-h2">Log New Expense</h2>
                </div>

                <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '24px' }}>
                    <div className="input-group">
                        <label className="text-label-caps">Transaction Amount (฿) *</label>
                        <div style={{ position: 'relative' }}>
                            <DollarSign size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                            <input 
                                className="input-field" 
                                style={{ paddingLeft: '40px', marginBottom: 0 }}
                                type="number" 
                                step="0.01" 
                                required 
                                value={formData.amount} 
                                onChange={e => setFormData({...formData, amount: e.target.value})} 
                                placeholder="0.00"
                            />
                        </div>
                    </div>
                    
                    <div className="input-group">
                        <label className="text-label-caps">Expense Category *</label>
                        <div style={{ position: 'relative' }}>
                            <Tag size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                            <select 
                                className="input-field" 
                                style={{ paddingLeft: '40px', marginBottom: 0 }}
                                required 
                                value={formData.category} 
                                onChange={e => setFormData({...formData, category: e.target.value})}
                            >
                                <option value="material_cost">Materials & Inventory</option>
                                <option value="labor_cost">Labor & Wages</option>
                                <option value="sub_contract_cost">Subcontractor Payment</option>
                                <option value="machinery_cost">Machinery & Tools</option>
                                <option value="other_cost">Other Miscellaneous</option>
                            </select>
                        </div>
                    </div>

                    <div className="input-group">
                        <label className="text-label-caps">Transaction Date *</label>
                        <div style={{ position: 'relative' }}>
                            <Calendar size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                            <input 
                                className="input-field" 
                                style={{ paddingLeft: '40px', marginBottom: 0 }}
                                type="date" 
                                required 
                                value={formData.date} 
                                onChange={e => setFormData({...formData, date: e.target.value})} 
                            />
                        </div>
                    </div>

                    <div className="input-group">
                        <label className="text-label-caps">Description / Note</label>
                        <div style={{ position: 'relative' }}>
                            <FileText size={18} style={{ position: 'absolute', left: '12px', top: '16px', color: 'var(--text-muted)' }} />
                            <textarea 
                                className="input-field" 
                                style={{ paddingLeft: '40px', minHeight: '100px', resize: 'vertical' }}
                                value={formData.note} 
                                onChange={e => setFormData({...formData, note: e.target.value})} 
                                placeholder="e.g. Purchased 50 bags of cement..."
                            />
                        </div>
                    </div>

                    <button type="submit" disabled={loading} className="btn-primary" style={{ marginTop: '16px' }}>
                        {loading ? <div className="spinner-small" style={{ borderTopColor: 'white' }}></div> : <><Check size={20} /> Save Expense Record</>}
                    </button>
                </form>
            </div>
        </div>
    );
}

export default AddExpense;
