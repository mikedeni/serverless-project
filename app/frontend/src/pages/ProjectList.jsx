import React, { useEffect, useState } from 'react';
import api from '../utils/api';
import ProjectSummaryCard from '../components/ProjectSummaryCard';
import { Link } from 'react-router-dom';
import { Plus, Search, Filter } from 'lucide-react';

const ProjectList = () => {
    const [projects, setProjects] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const data = await api.get('/projects');
                setProjects(data.items || []);
            } catch (err) {
                console.error("Error loading projects", err);
            } finally {
                setLoading(false);
            }
        };
        fetchProjects();
    }, []);

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>Management</p>
                    <h1 className="text-h1">All Projects</h1>
                </div>
                <Link to="/projects/new" className="btn-primary" style={{ textDecoration: 'none' }}>
                    <Plus size={20} /> Create New Project
                </Link>
            </header>

            <div className="glass-panel" style={{ padding: '16px', marginBottom: '32px', display: 'flex', gap: '16px', alignItems: 'center' }}>
                <div style={{ position: 'relative', flex: 1 }}>
                    <Search size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                    <input 
                        type="text" 
                        placeholder="Search projects..." 
                        className="input-field" 
                        style={{ paddingLeft: '40px', marginBottom: 0 }}
                    />
                </div>
                <button className="btn-secondary" style={{ width: 'auto' }}>
                    <Filter size={18} /> Filters
                </button>
            </div>

            {loading ? (
                <div className="grid">
                    {[1,2,3,4].map(i => <div key={i} className="skeleton-loader" style={{ height: '200px' }}></div>)}
                </div>
            ) : projects.length === 0 ? (
                <div className="glass-panel" style={{ padding: '80px', textAlign: 'center' }}>
                    <h2 className="text-h3" style={{ color: 'var(--text-muted)', marginBottom: '16px' }}>No Projects Found</h2>
                    <p style={{ marginBottom: '24px', color: 'var(--text-muted)' }}>Get started by creating your first construction project.</p>
                    <Link to="/projects/new" className="btn-primary" style={{ display: 'inline-flex', width: 'auto' }}>Create Project</Link>
                </div>
            ) : (
                <div className="grid">
                    {projects.map(p => (
                        <ProjectSummaryCard key={p.id} project={p} />
                    ))}
                </div>
            )}
        </div>
    )
};

export default ProjectList;
