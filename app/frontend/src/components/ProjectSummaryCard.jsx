import React from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight, Activity } from 'lucide-react';

export default function ProjectSummaryCard({ project }) {
    let progressPercentage = 0;
    if (project.status === 'completed') progressPercentage = 100;
    else if (project.status === 'active') progressPercentage = 65;
    else progressPercentage = 10;

    const totalSpent = project.totalSpent || 0;
    const budgetUsedPercentage = project.budget > 0 ? Math.round((totalSpent / project.budget) * 100) : 0;

    return (
        <div className="glass-panel interactive-card" style={{ padding: '24px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '16px' }}>
                <div style={{ width: '48px', height: '48px', borderRadius: '12px', overflow: 'hidden', background: 'var(--surface-variant)' }}>
                    {project.imageUrl ? (
                        <img src={project.imageUrl} alt={project.projectName} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                    ) : (
                        <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'rgba(37, 99, 235, 0.1)' }}>
                            <Activity size={24} color="var(--secondary)" />
                        </div>
                    )}
                </div>
                <span className="status-badge" style={{ 
                    background: project.status === 'active' ? 'rgba(16, 185, 129, 0.1)' : 'rgba(148, 163, 184, 0.1)',
                    color: project.status === 'active' ? 'var(--success)' : 'var(--text-muted)'
                }}>
                    {project.status}
                </span>
            </div>

            <h3 className="text-h3" style={{ marginBottom: '16px', fontSize: '18px' }}>{project.projectName}</h3>
            
            <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '13px', color: 'var(--text-muted)', marginBottom: '8px' }}>
                <span>Budget Usage</span>
                <span>{budgetUsedPercentage}%</span>
            </div>
            
            <div style={{ height: '8px', background: 'var(--surface-variant)', borderRadius: '4px', overflow: 'hidden', marginBottom: '24px' }}>
                <div 
                    style={{ 
                        height: '100%', 
                        width: `${budgetUsedPercentage}%`, 
                        background: budgetUsedPercentage > 90 ? 'var(--error)' : 'var(--secondary)',
                        transition: 'width 0.5s ease'
                    }}
                ></div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ fontSize: '10px' }}>Total Spent</p>
                    <p style={{ fontWeight: 700, color: 'var(--on-surface)' }}>฿{totalSpent.toLocaleString()}</p>
                </div>
                <Link to={`/projects/${project.id}`} className="btn-secondary" style={{ padding: '8px 12px', fontSize: '12px', textDecoration: 'none' }}>
                    Details <ArrowRight size={14} />
                </Link>
            </div>
        </div>
    );
}
