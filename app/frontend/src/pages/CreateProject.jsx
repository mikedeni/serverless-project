import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../utils/api';
import { 
    Plus, 
    Calendar, 
    DollarSign, 
    Briefcase, 
    ArrowLeft, 
    Check,
    Info
} from 'lucide-react';

const CreateProject = () => {
    const [formData, setFormData] = useState({
        projectName: '',
        startDate: '',
        endDate: '',
        budget: 0,
        status: 'planning'
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: name === 'budget' ? parseFloat(value) || 0 : value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            await api.post('/projects', formData);
            navigate('/projects');
        } catch (err) {
            setError(err.message || 'Failed to create project');
            setLoading(false);
        }
    };

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
            <div className="glass-panel fadeIn" style={{ width: '100%', maxWidth: '600px', padding: '48px' }}>
                <Link to="/dashboard" className="btn-secondary" style={{ width: 'auto', marginBottom: '32px', textDecoration: 'none', border: 'none', background: 'none', padding: 0, color: 'var(--text-muted)' }}>
                    <ArrowLeft size={18} /> Back to Dashboard
                </Link>
                
                <div style={{ display: 'flex', alignItems: 'center', gap: '16px', marginBottom: '40px' }}>
                    <div style={{ background: 'var(--primary)', color: 'white', padding: '12px', borderRadius: '12px' }}>
                        <Briefcase size={28} />
                    </div>
                    <div>
                        <h2 className="text-h2">Create New Project</h2>
                        <p className="text-body-sm" style={{ color: 'var(--on-surface-variant)' }}>Set up a new construction workspace and budget.</p>
                    </div>
                </div>

                <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '28px' }}>
                    {error && (
                        <div className="auth-error" style={{ background: 'var(--error-container)', color: 'var(--on-error-container)', padding: '12px', borderRadius: '8px' }}>
                            {error}
                        </div>
                    )}
                    
                    <div className="input-group">
                        <label className="text-label-caps">Project Name *</label>
                        <div style={{ position: 'relative' }}>
                            <Info size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                            <input 
                                className="input-field"
                                style={{ paddingLeft: '40px', marginBottom: 0 }}
                                type="text" 
                                name="projectName" 
                                required 
                                value={formData.projectName}
                                onChange={handleChange}
                                placeholder="e.g. Riverside Villa Residence"
                            />
                        </div>
                    </div>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
                        <div className="input-group">
                            <label className="text-label-caps">Start Date</label>
                            <div style={{ position: 'relative' }}>
                                <Calendar size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                                <input 
                                    className="input-field"
                                    style={{ paddingLeft: '40px', marginBottom: 0 }}
                                    type="date" 
                                    name="startDate" 
                                    required 
                                    value={formData.startDate}
                                    onChange={handleChange}
                                />
                            </div>
                        </div>
                        <div className="input-group">
                            <label className="text-label-caps">Target Completion</label>
                            <div style={{ position: 'relative' }}>
                                <Calendar size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                                <input 
                                    className="input-field"
                                    style={{ paddingLeft: '40px', marginBottom: 0 }}
                                    type="date" 
                                    name="endDate" 
                                    required 
                                    value={formData.endDate}
                                    onChange={handleChange}
                                />
                            </div>
                        </div>
                    </div>

                    <div className="input-group">
                        <label className="text-label-caps">Project Budget (฿)</label>
                        <div style={{ position: 'relative' }}>
                            <DollarSign size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                            <input 
                                className="input-field"
                                style={{ paddingLeft: '40px', marginBottom: 0 }}
                                type="number" 
                                name="budget" 
                                required 
                                value={formData.budget}
                                onChange={handleChange}
                                placeholder="0.00"
                            />
                        </div>
                    </div>

                    <div className="input-group">
                        <label className="text-label-caps">Initial Status</label>
                        <select 
                            className="input-field"
                            name="status" 
                            value={formData.status}
                            onChange={handleChange}
                        >
                            <option value="planning">Phase 1: Planning</option>
                            <option value="active">Phase 2: Construction Active</option>
                            <option value="on_hold">Phase 3: On Hold / Paused</option>
                        </select>
                    </div>

                    <button type="submit" className="btn-primary" style={{ marginTop: '16px', height: '52px' }} disabled={loading}>
                        {loading ? <div className="spinner-small" style={{ borderTopColor: 'white' }}></div> : <><Plus size={20} /> Initialize Project</>}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default CreateProject;
