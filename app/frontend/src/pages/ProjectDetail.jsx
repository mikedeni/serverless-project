import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import api from '../utils/api';
import { 
    Briefcase, 
    Edit, 
    Trash2, 
    Plus, 
    FileText, 
    Receipt, 
    BarChart2, 
    ClipboardList, 
    Folder, 
    ChevronLeft, 
    Calendar, 
    DollarSign, 
    Info,
    X,
    Check,
    TrendingUp,
    TrendingDown,
    Activity,
    Camera
} from 'lucide-react';
import ImageUpload from '../components/ImageUpload';

const ProjectDetail = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [budgetSummary, setBudgetSummary] = useState(null);
    const [expenses, setExpenses] = useState([]);
    const [project, setProject] = useState(null);
    const [loading, setLoading] = useState(true);
    
    const [showEditForm, setShowEditForm] = useState(false);
    const [formData, setFormData] = useState({ projectName: '', budget: 0, status: 'active' });

    useEffect(() => {
        fetchData();
    }, [id]);

    const fetchData = async () => {
        try {
            setLoading(true);
            const summaryRes = await api.get(`/expenses/project/${id}/budget-summary`);
            const expensesRes = await api.get(`/expenses/project/${id}`);
            const projectRes = await api.get(`/projects/${id}`);
            
            setBudgetSummary(summaryRes);
            setExpenses(expensesRes.items || []);
            setProject(projectRes);
            setFormData({ projectName: projectRes.projectName, budget: projectRes.budget, status: projectRes.status });
        } catch (e) {
            console.error("Error fetching detail", e);
        } finally {
            setLoading(false);
        }
    };

    const handleEditProject = async (e) => {
        e.preventDefault();
        try {
            await api.put(`/projects/${id}`, formData);
            setShowEditForm(false);
            fetchData();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleDeleteProject = async () => {
        if (!confirm('Are you sure you want to delete this project? This action cannot be undone.')) return;
        try {
            await api.delete(`/projects/${id}`);
            navigate('/projects');
        } catch (err) {
            alert(err.message);
        }
    };

    if (loading) return (
        <div className="loader-container">
            <div className="spinner"></div>
        </div>
    );

    const isBudgetExceeded = (budgetSummary?.remainingBudget || 0) < 0;

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
               <div>
                 <Link to="/projects" className="btn-secondary" style={{ width: 'auto', marginBottom: '16px', textDecoration: 'none' }}>
                    <ChevronLeft size={18} /> Back to Projects
                 </Link>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '20px' }}>
                    <ImageUpload 
                        entityType="projects" 
                        entityId={id} 
                        currentImage={project?.imageUrl} 
                        onUploadSuccess={(url) => setProject(prev => ({ ...prev, imageUrl: url }))}
                    />
                    <div>
                        <h1 className="text-h1" style={{ fontSize: '32px' }}>{project?.projectName || `Project #${id}`}</h1>
                        <span className="status-badge" style={{ 
                            background: project?.status === 'active' ? 'rgba(16, 185, 129, 0.1)' : 'var(--surface-variant)',
                            color: project?.status === 'active' ? 'var(--success)' : 'var(--text-muted)'
                        }}>
                            {project?.status.replace('_', ' ')}
                        </span>
                    </div>
                  </div>
               </div>
               <div style={{ display: 'flex', gap: '12px' }}>
                 <button onClick={() => setShowEditForm(true)} className="btn-secondary" style={{ width: 'auto' }}>
                    <Edit size={18} /> Edit
                 </button>
                 <button onClick={handleDeleteProject} className="btn-secondary" style={{ width: 'auto', color: 'var(--error)' }}>
                    <Trash2 size={18} />
                 </button>
                 <Link to={`/projects/${id}/expenses/add`} className="btn-primary" style={{ textDecoration: 'none' }}>
                    <Plus size={20} /> Log Expense
                 </Link>
               </div>
            </header>

            {/* Quick Access Grid */}
            <div className="grid" style={{ gridTemplateColumns: 'repeat(auto-fit, minmax(160px, 1fr))', gap: '20px', marginBottom: '40px' }}>
                {[
                    { label: 'BOQ', icon: ClipboardList, path: `/projects/${id}/quotations`, color: '#6366F1' },
                    { label: 'Invoices', icon: Receipt, path: `/projects/${id}/invoices`, color: '#F59E0B' },
                    { label: 'Fin. Report', icon: BarChart2, path: `/projects/${id}/report`, color: '#10B981' },
                    { label: 'Daily Log', icon: FileText, path: `/projects/${id}/daily-reports`, color: '#3B82F6' },
                    { label: 'Docs', icon: Folder, path: `/projects/${id}/documents`, color: '#64748B' },
                ].map((item, idx) => (
                    <Link key={idx} to={item.path} className="glass-panel interactive-card" style={{ padding: '24px', textAlign: 'center', textDecoration: 'none', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '12px' }}>
                        <div style={{ width: '48px', height: '48px', borderRadius: '12px', background: `${item.color}15`, display: 'flex', alignItems: 'center', justifyContent: 'center', color: item.color }}>
                            <item.icon size={24} />
                        </div>
                        <span style={{ fontWeight: 600, color: 'var(--on-surface)', fontSize: '14px' }}>{item.label}</span>
                    </Link>
                ))}
            </div>

            {showEditForm && (
                <div className="glass-panel fadeIn" style={{ padding: '32px', marginBottom: '40px', borderLeft: '4px solid var(--secondary)' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                        <h3 className="text-h3">Edit Project Details</h3>
                        <button onClick={() => setShowEditForm(false)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)' }}><X size={24} /></button>
                    </div>
                    <form onSubmit={handleEditProject}>
                        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '24px' }}>
                            <div className="input-group">
                                <label className="text-label-caps">Project Name *</label>
                                <input className="input-field" required value={formData.projectName} onChange={(e) => setFormData({ ...formData, projectName: e.target.value })} />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Total Budget (฿) *</label>
                                <input className="input-field" type="number" min="0" required value={formData.budget} onChange={(e) => setFormData({ ...formData, budget: parseFloat(e.target.value) || 0 })} />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Project Status</label>
                                <select className="input-field" value={formData.status} onChange={(e) => setFormData({ ...formData, status: e.target.value })}>
                                    <option value="active">Active</option>
                                    <option value="completed">Completed</option>
                                    <option value="on_hold">On Hold</option>
                                </select>
                            </div>
                        </div>
                        <div style={{ display: 'flex', gap: '16px', marginTop: '32px' }}>
                            <button type="submit" className="btn-primary" style={{ width: 'auto' }}><Check size={20} /> Save Changes</button>
                            <button type="button" onClick={() => setShowEditForm(false)} className="btn-secondary" style={{ width: 'auto' }}>Cancel</button>
                        </div>
                    </form>
                </div>
            )}

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 320px', gap: '40px', alignItems: 'start' }}>
                {/* Main Content: Expense History */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                        <Activity size={20} color="var(--primary)" />
                        <h2 className="text-h3" style={{ fontSize: '20px' }}>Expense History</h2>
                    </div>
                    <div className="glass-panel table-wrapper">
                        {expenses.length === 0 ? (
                            <div style={{ padding: '60px', textAlign: 'center' }}>
                                <Receipt size={40} color="var(--outline-variant)" style={{ marginBottom: '16px' }} />
                                <p style={{ color: 'var(--text-muted)' }}>No expenses recorded yet.</p>
                                <Link to={`/projects/${id}/expenses/add`} style={{ color: 'var(--secondary)', fontWeight: 600, fontSize: '14px', marginTop: '8px', display: 'inline-block' }}>Log your first expense</Link>
                            </div>
                        ) : (
                          <table className="modern-table">
                              <thead>
                                  <tr>
                                      <th>Date</th>
                                      <th>Category</th>
                                      <th>Note</th>
                                      <th style={{ textAlign: 'right' }}>Amount</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  {expenses.map(exp => (
                                      <tr key={exp.id}>
                                          <td><div style={{ display: 'flex', alignItems: 'center', gap: '8px', color: 'var(--text-muted)' }}><Calendar size={14} /> {new Date(exp.date).toLocaleDateString()}</div></td>
                                          <td><span className="status-badge" style={{ background: 'var(--surface-variant)', color: 'var(--on-surface-variant)', fontSize: '10px' }}>{exp.category.replace('_cost', '')}</span></td>
                                          <td style={{ fontSize: '14px' }}>{exp.note || '-'}</td>
                                          <td style={{ textAlign: 'right', color: 'var(--error)', fontWeight: 700 }}>฿{exp.amount.toLocaleString()}</td>
                                      </tr>
                                  ))}
                              </tbody>
                          </table>
                        )}
                    </div>
                </div>

                {/* Sidebar Content: Budget Summary */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                        <TrendingUp size={20} color="var(--primary)" />
                        <h2 className="text-h3" style={{ fontSize: '20px' }}>Financial Summary</h2>
                    </div>
                    <div className="glass-panel" style={{ padding: '28px' }}>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                            <div>
                                <p className="text-label-caps" style={{ fontSize: '10px', marginBottom: '4px' }}>Allocated Budget</p>
                                <div style={{ display: 'flex', alignItems: 'baseline', gap: '4px' }}>
                                    <span style={{ fontSize: '14px', color: 'var(--text-muted)' }}>฿</span>
                                    <span style={{ fontSize: '24px', fontWeight: 700 }}>{(budgetSummary?.totalBudget || 0).toLocaleString()}</span>
                                </div>
                            </div>

                            <div>
                                <p className="text-label-caps" style={{ fontSize: '10px', marginBottom: '4px' }}>Actual Spent</p>
                                <div style={{ display: 'flex', alignItems: 'baseline', gap: '4px', color: 'var(--error)' }}>
                                    <span style={{ fontSize: '14px' }}>฿</span>
                                    <span style={{ fontSize: '24px', fontWeight: 700 }}>{(budgetSummary?.totalExpenses || 0).toLocaleString()}</span>
                                </div>
                            </div>

                            <div style={{ padding: '16px', background: isBudgetExceeded ? 'var(--error-container)' : 'var(--secondary-container)', borderRadius: '12px', border: '1px solid var(--outline)' }}>
                                <p className="text-label-caps" style={{ fontSize: '10px', marginBottom: '4px', color: isBudgetExceeded ? 'var(--on-error-container)' : 'var(--primary)' }}>Remaining Balance</p>
                                <div style={{ display: 'flex', alignItems: 'baseline', gap: '4px', color: isBudgetExceeded ? 'var(--error)' : 'var(--primary)' }}>
                                    <span style={{ fontSize: '14px' }}>฿</span>
                                    <span style={{ fontSize: '24px', fontWeight: 800 }}>{(budgetSummary?.remainingBudget || 0).toLocaleString()}</span>
                                    {isBudgetExceeded ? <TrendingDown size={18} style={{ marginLeft: '4px' }} /> : <TrendingUp size={18} style={{ marginLeft: '4px' }} />}
                                </div>
                            </div>

                            <div>
                                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                                    <span className="text-label-caps" style={{ fontSize: '10px' }}>Budget Utilization</span>
                                    <span style={{ fontWeight: 700, fontSize: '12px' }}>{budgetSummary?.variancePercentage || 0}%</span>
                                </div>
                                <div style={{ height: '8px', background: 'var(--surface-variant)', borderRadius: '4px', overflow: 'hidden' }}>
                                    <div 
                                        style={{ 
                                            height: '100%', 
                                            width: `${Math.min(budgetSummary?.variancePercentage || 0, 100)}%`, 
                                            background: isBudgetExceeded ? 'var(--error)' : 'var(--primary)',
                                            transition: 'width 0.8s cubic-bezier(0.4, 0, 0.2, 1)'
                                        }}
                                    ></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProjectDetail;
