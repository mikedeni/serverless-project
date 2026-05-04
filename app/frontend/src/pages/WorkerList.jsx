import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../utils/api';
import { 
    Users, 
    UserPlus, 
    Edit, 
    Trash2, 
    Search, 
    Phone, 
    Briefcase, 
    Banknote,
    X,
    Check,
    Camera
} from 'lucide-react';
import ImageUpload from '../components/ImageUpload';

const WorkerList = () => {
    const [workers, setWorkers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState('');
    const [filterStatus, setFilterStatus] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [editingWorker, setEditingWorker] = useState(null);
    const [formData, setFormData] = useState({ name: '', position: '', dailyWage: 0, phone: '', status: 'active' });

    useEffect(() => {
        fetchWorkers();
    }, [search, filterStatus]);

    const fetchWorkers = async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams();
            if (search) params.append('search', search);
            if (filterStatus) params.append('status', filterStatus);
            const data = await api.get(`/workers?${params.toString()}`);
            setWorkers(data.items || []);
        } catch (err) {
            console.error('Failed to load workers', err);
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingWorker) {
                await api.put(`/workers/${editingWorker.id}`, formData);
            } else {
                await api.post('/workers', formData);
            }
            setShowForm(false);
            setEditingWorker(null);
            setFormData({ name: '', position: '', dailyWage: 0, phone: '', status: 'active' });
            fetchWorkers();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleEdit = (worker) => {
        setEditingWorker(worker);
        setFormData({ name: worker.name, position: worker.position || '', dailyWage: worker.dailyWage, phone: worker.phone || '', status: worker.status });
        setShowForm(true);
    };

    const handleDelete = async (id) => {
        if (!confirm('ลบแรงงานนี้?')) return;
        try {
            await api.delete(`/workers/${id}`);
            fetchWorkers();
        } catch (err) {
            alert(err.message);
        }
    };

    const cancelForm = () => {
        setShowForm(false);
        setEditingWorker(null);
        setFormData({ name: '', position: '', dailyWage: 0, phone: '', status: 'active' });
    };

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>Human Resources</p>
                    <h1 className="text-h1">Workforce</h1>
                </div>
                <button onClick={() => { cancelForm(); setShowForm(true); }} className="btn-primary">
                    <UserPlus size={20} /> Add Worker
                </button>
            </header>

            {/* Search & Filter */}
            <div className="glass-panel" style={{ padding: '16px', marginBottom: '32px', display: 'flex', gap: '16px', alignItems: 'center' }}>
                <div style={{ position: 'relative', flex: 1 }}>
                    <Search size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                    <input 
                        className="input-field"
                        style={{ paddingLeft: '40px', marginBottom: 0 }}
                        placeholder="Search by name or position..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                    />
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                    <button onClick={() => setFilterStatus('')} className={`btn-secondary ${!filterStatus ? 'active' : ''}`} style={{ width: 'auto', padding: '8px 16px' }}>All</button>
                    <button onClick={() => setFilterStatus('active')} className={`btn-secondary ${filterStatus === 'active' ? 'active' : ''}`} style={{ width: 'auto', padding: '8px 16px', color: filterStatus === 'active' ? 'var(--success)' : '' }}>Active</button>
                    <button onClick={() => setFilterStatus('inactive')} className={`btn-secondary ${filterStatus === 'inactive' ? 'active' : ''}`} style={{ width: 'auto', padding: '8px 16px', color: filterStatus === 'inactive' ? 'var(--error)' : '' }}>Inactive</button>
                </div>
            </div>

            {/* Add/Edit Form */}
            {showForm && (
                <div className="glass-panel fadeIn" style={{ padding: '32px', marginBottom: '32px', borderLeft: '4px solid var(--secondary)' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                        <h3 className="text-h3">{editingWorker ? 'Edit Worker Profile' : 'Register New Worker'}</h3>
                        <button onClick={cancelForm} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)' }}><X size={24} /></button>
                    </div>
                    <form onSubmit={handleSubmit}>
                        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '24px' }}>
                            <div className="input-group">
                                <label className="text-label-caps">Full Name *</label>
                                <input className="input-field" required value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} placeholder="Worker Name" />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Position / Skill</label>
                                <input className="input-field" value={formData.position} onChange={(e) => setFormData({ ...formData, position: e.target.value })} placeholder="e.g. Mason, Plumber" />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Daily Wage (฿)</label>
                                <input className="input-field" type="number" min="0" required value={formData.dailyWage} onChange={(e) => setFormData({ ...formData, dailyWage: parseFloat(e.target.value) || 0 })} />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Contact Phone</label>
                                <input className="input-field" value={formData.phone} onChange={(e) => setFormData({ ...formData, phone: e.target.value })} placeholder="08x-xxx-xxxx" />
                            </div>
                        </div>

                        {editingWorker && (
                            <div style={{ marginTop: '24px', display: 'flex', alignItems: 'center', gap: '20px' }}>
                                <label className="text-label-caps" style={{ marginBottom: 0 }}>Worker Photo</label>
                                <ImageUpload 
                                    entityType="workers" 
                                    entityId={editingWorker.id} 
                                    currentImage={editingWorker.imageUrl} 
                                    onUploadSuccess={(url) => {
                                        setEditingWorker(prev => ({ ...prev, imageUrl: url }));
                                        fetchWorkers();
                                    }}
                                />
                            </div>
                        )}
                        <div style={{ display: 'flex', gap: '16px', marginTop: '32px' }}>
                            <button type="submit" className="btn-primary" style={{ width: 'auto' }}>
                                <Check size={20} /> {editingWorker ? 'Update Profile' : 'Confirm Registration'}
                            </button>
                            <button type="button" onClick={cancelForm} className="btn-secondary" style={{ width: 'auto' }}>Cancel</button>
                        </div>
                    </form>
                </div>
            )}

            {/* Workers Grid/Table */}
            {loading ? (
                <div className="grid">
                    {[1,2,3,4].map(i => <div key={i} className="skeleton-loader" style={{ height: '100px' }}></div>)}
                </div>
            ) : workers.length === 0 ? (
                <div className="glass-panel" style={{ padding: '80px', textAlign: 'center' }}>
                    <Users size={48} color="var(--outline-variant)" style={{ marginBottom: '16px' }} />
                    <h2 className="text-h3" style={{ color: 'var(--text-muted)' }}>No Workforce Records</h2>
                    <p style={{ color: 'var(--text-muted)' }}>Start by adding workers to your company database.</p>
                </div>
            ) : (
                <div className="glass-panel table-wrapper">
                    <table className="modern-table">
                        <thead>
                            <tr>
                                <th>Worker Details</th>
                                <th>Position</th>
                                <th>Daily Rate</th>
                                <th>Contact</th>
                                <th>Status</th>
                                <th style={{ textAlign: 'right' }}>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {workers.map(w => (
                                <tr key={w.id}>
                                    <td>
                                        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                            {w.imageUrl ? (
                                                <img 
                                                    src={w.imageUrl} 
                                                    alt={w.name} 
                                                    style={{ width: '36px', height: '36px', borderRadius: '50%', objectFit: 'cover' }} 
                                                />
                                            ) : (
                                                <div style={{ width: '36px', height: '36px', background: 'var(--primary-container)', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--primary)', fontWeight: 700 }}>
                                                    {w.name.charAt(0)}
                                                </div>
                                            )}
                                            <span style={{ fontWeight: 600 }}>{w.name}</span>
                                        </div>
                                    </td>
                                    <td>
                                        <div style={{ display: 'flex', alignItems: 'center', gap: '8px', color: 'var(--on-surface-variant)' }}>
                                            <Briefcase size={14} /> {w.position || '-'}
                                        </div>
                                    </td>
                                    <td>
                                        <div style={{ display: 'flex', alignItems: 'center', gap: '8px', color: 'var(--success)', fontWeight: 600 }}>
                                            <Banknote size={14} /> ฿{w.dailyWage.toLocaleString()}
                                        </div>
                                    </td>
                                    <td>
                                        <div style={{ display: 'flex', alignItems: 'center', gap: '8px', color: 'var(--text-muted)' }}>
                                            <Phone size={14} /> {w.phone || '-'}
                                        </div>
                                    </td>
                                    <td>
                                        <span className="status-badge" style={{
                                            background: w.status === 'active' ? 'rgba(16, 185, 129, 0.1)' : 'rgba(186, 26, 26, 0.1)',
                                            color: w.status === 'active' ? 'var(--success)' : 'var(--error)'
                                        }}>
                                            {w.status}
                                        </span>
                                    </td>
                                    <td>
                                        <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                                            <button onClick={() => handleEdit(w)} className="btn-secondary" style={{ padding: '6px 10px' }} title="Edit">
                                                <Edit size={14} />
                                            </button>
                                            <button onClick={() => handleDelete(w.id)} className="btn-secondary" style={{ padding: '6px 10px', color: 'var(--error)' }} title="Delete">
                                                <Trash2 size={14} />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default WorkerList;
