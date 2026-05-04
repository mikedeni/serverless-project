import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../utils/api';
import { 
    Package, 
    AlertTriangle, 
    Search, 
    Plus, 
    Edit, 
    Trash2, 
    Eye, 
    ArrowRight,
    X,
    Check,
    Info,
    Camera
} from 'lucide-react';
import ImageUpload from '../components/ImageUpload';

const MaterialList = () => {
    const [materials, setMaterials] = useState([]);
    const [lowStockMaterials, setLowStockMaterials] = useState([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [editingMaterial, setEditingMaterial] = useState(null);
    const [formData, setFormData] = useState({ name: '', unit: '', minStock: 0 });

    useEffect(() => {
        fetchMaterials();
        fetchLowStock();
    }, [search]);

    const fetchMaterials = async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams();
            if (search) params.append('search', search);
            const data = await api.get(`/materials?${params.toString()}`);
            setMaterials(data.items || []);
        } catch (err) {
            console.error('Failed to load materials', err);
        } finally {
            setLoading(false);
        }
    };

    const fetchLowStock = async () => {
        try {
            const data = await api.get('/materials/low-stock');
            setLowStockMaterials(data || []);
        } catch (err) {
            console.error('Failed to load low stock', err);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingMaterial) {
                await api.put(`/materials/${editingMaterial.id}`, formData);
            } else {
                await api.post('/materials', formData);
            }
            setShowForm(false);
            setEditingMaterial(null);
            setFormData({ name: '', unit: '', minStock: 0 });
            fetchMaterials();
            fetchLowStock();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleEdit = (m) => {
        setEditingMaterial(m);
        setFormData({ name: m.name, unit: m.unit, minStock: m.minStock });
        setShowForm(true);
    };

    const handleDelete = async (id) => {
        if (!confirm('ลบวัสดุนี้?')) return;
        try {
            await api.delete(`/materials/${id}`);
            fetchMaterials();
            fetchLowStock();
        } catch (err) {
            alert(err.message);
        }
    };

    const cancelForm = () => {
        setShowForm(false);
        setEditingMaterial(null);
        setFormData({ name: '', unit: '', minStock: 0 });
    };

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>Inventory Management</p>
                    <h1 className="text-h1">Materials & Stock</h1>
                </div>
                <button onClick={() => { cancelForm(); setShowForm(true); }} className="btn-primary">
                    <Plus size={20} /> Add Material
                </button>
            </header>

            {/* Low Stock Alert Bar */}
            {lowStockMaterials.length > 0 && (
                <div className="glass-panel fadeIn" style={{ padding: '16px 24px', marginBottom: '32px', borderLeft: '4px solid var(--warning)', background: 'rgba(245, 158, 11, 0.05)', display: 'flex', alignItems: 'center', gap: '20px' }}>
                    <div style={{ background: 'var(--warning)', color: 'white', padding: '8px', borderRadius: '8px' }}>
                        <AlertTriangle size={20} />
                    </div>
                    <div style={{ flex: 1 }}>
                        <p style={{ fontWeight: 700, color: 'var(--on-surface)', marginBottom: '4px' }}>Low Stock Alert</p>
                        <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
                            {lowStockMaterials.map(m => (
                                <span key={m.id} className="status-badge" style={{ background: 'rgba(245, 158, 11, 0.1)', color: 'var(--warning)', fontSize: '11px' }}>
                                    {m.name} ({m.currentStock} {m.unit})
                                </span>
                            ))}
                        </div>
                    </div>
                </div>
            )}

            {/* Search Bar */}
            <div className="glass-panel" style={{ padding: '16px', marginBottom: '32px', display: 'flex', gap: '16px' }}>
                <div style={{ position: 'relative', flex: 1 }}>
                    <Search size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                    <input 
                        className="input-field"
                        style={{ paddingLeft: '40px', marginBottom: 0 }}
                        placeholder="Search materials by name..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                    />
                </div>
            </div>

            {/* Add/Edit Form Overlay-like Panel */}
            {showForm && (
                <div className="glass-panel fadeIn" style={{ padding: '32px', marginBottom: '32px', borderLeft: '4px solid var(--secondary)' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                        <h3 className="text-h3">{editingMaterial ? 'Update Material Details' : 'Register New Material'}</h3>
                        <button onClick={cancelForm} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)' }}><X size={24} /></button>
                    </div>
                    <form onSubmit={handleSubmit}>
                        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '24px' }}>
                            <div className="input-group">
                                <label className="text-label-caps">Material Name *</label>
                                <input className="input-field" required value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} placeholder="e.g. Portland Cement" />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Unit of Measurement *</label>
                                <input className="input-field" required value={formData.unit} onChange={(e) => setFormData({ ...formData, unit: e.target.value })} placeholder="e.g. Bag, Cubic Meter, Sheet" />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Minimum Stock Alert Threshold</label>
                                <input className="input-field" type="number" min="0" value={formData.minStock} onChange={(e) => setFormData({ ...formData, minStock: parseFloat(e.target.value) || 0 })} />
                            </div>
                        </div>

                        {editingMaterial && (
                            <div style={{ marginTop: '24px', display: 'flex', alignItems: 'center', gap: '20px' }}>
                                <label className="text-label-caps" style={{ marginBottom: 0 }}>Material Photo</label>
                                <ImageUpload 
                                    entityType="materials" 
                                    entityId={editingMaterial.id} 
                                    currentImage={editingMaterial.imageUrl} 
                                    onUploadSuccess={(url) => {
                                        setEditingMaterial(prev => ({ ...prev, imageUrl: url }));
                                        fetchMaterials();
                                    }}
                                />
                            </div>
                        )}
                        <div style={{ display: 'flex', gap: '16px', marginTop: '32px' }}>
                            <button type="submit" className="btn-primary" style={{ width: 'auto' }}>
                                <Check size={20} /> {editingMaterial ? 'Save Changes' : 'Add to Inventory'}
                            </button>
                            <button type="button" onClick={cancelForm} className="btn-secondary" style={{ width: 'auto' }}>Cancel</button>
                        </div>
                    </form>
                </div>
            )}

            {/* Materials Data Table */}
            {loading ? (
                <div className="grid">
                    {[1,2,3,4].map(i => <div key={i} className="skeleton-loader" style={{ height: '100px' }}></div>)}
                </div>
            ) : materials.length === 0 ? (
                <div className="glass-panel" style={{ padding: '80px', textAlign: 'center' }}>
                    <Package size={48} color="var(--outline-variant)" style={{ marginBottom: '16px' }} />
                    <p className="text-h3" style={{ color: 'var(--text-muted)' }}>Inventory is empty</p>
                    <p style={{ color: 'var(--text-muted)' }}>Add materials to track your stock levels and pricing history.</p>
                </div>
            ) : (
                <div className="glass-panel table-wrapper">
                    <table className="modern-table">
                        <thead>
                            <tr>
                                <th>Material Name</th>
                                <th>Unit</th>
                                <th>Stock Level</th>
                                <th>Last Price</th>
                                <th>Status</th>
                                <th style={{ textAlign: 'right' }}>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {materials.map(m => {
                                const isLow = m.minStock > 0 && m.currentStock <= m.minStock;
                                return (
                                    <tr key={m.id}>
                                        <td>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                                {m.imageUrl ? (
                                                    <img 
                                                        src={m.imageUrl} 
                                                        alt={m.name} 
                                                        style={{ width: '36px', height: '36px', borderRadius: '8px', objectFit: 'cover' }} 
                                                    />
                                                ) : (
                                                    <div style={{ width: '36px', height: '36px', background: 'var(--surface-variant)', borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--primary)' }}>
                                                        <Package size={18} />
                                                    </div>
                                                )}
                                                <Link to={`/materials/${m.id}`} style={{ fontWeight: 600, color: 'var(--on-surface)', textDecoration: 'none' }}>{m.name}</Link>
                                            </div>
                                        </td>
                                        <td style={{ color: 'var(--text-muted)' }}>{m.unit}</td>
                                        <td>
                                            <div style={{ fontWeight: 700, color: isLow ? 'var(--error)' : 'var(--on-surface)' }}>
                                                {m.currentStock.toLocaleString()}
                                            </div>
                                            <p style={{ fontSize: '11px', color: 'var(--text-muted)' }}>Min: {m.minStock}</p>
                                        </td>
                                        <td style={{ color: 'var(--success)', fontWeight: 600 }}>
                                            {m.lastPrice > 0 ? `฿${m.lastPrice.toLocaleString()}` : '-'}
                                        </td>
                                        <td>
                                            {isLow ? (
                                                <span className="status-badge" style={{ background: 'rgba(245, 158, 11, 0.1)', color: 'var(--warning)' }}>Low Stock</span>
                                            ) : (
                                                <span className="status-badge" style={{ background: 'rgba(16, 185, 129, 0.1)', color: 'var(--success)' }}>Optimal</span>
                                            )}
                                        </td>
                                        <td>
                                            <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                                                <Link to={`/materials/${m.id}`} className="btn-secondary" style={{ padding: '6px 10px' }} title="View Detail">
                                                    <Eye size={14} />
                                                </Link>
                                                <button onClick={() => handleEdit(m)} className="btn-secondary" style={{ padding: '6px 10px' }} title="Edit">
                                                    <Edit size={14} />
                                                </button>
                                                <button onClick={() => handleDelete(m.id)} className="btn-secondary" style={{ padding: '6px 10px', color: 'var(--error)' }} title="Delete">
                                                    <Trash2 size={14} />
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default MaterialList;
