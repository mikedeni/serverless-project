import React, { useContext } from 'react';
import { NavLink } from 'react-router-dom';
import { 
  LayoutGrid, 
  DraftingCompass, 
  IdCard, 
  Calendar, 
  Package, 
  FileText, 
  Handshake, 
  Bell, 
  Settings,
  LogOut
} from 'lucide-react';
import { AuthContext } from '../contexts/AuthContext';

const Sidebar = () => {
  const { user, logout } = useContext(AuthContext);

  const navItems = [
    { to: '/dashboard', icon: LayoutGrid, label: 'Dashboard' },
    { to: '/projects', icon: DraftingCompass, label: 'Projects' },
    { to: '/workers', icon: IdCard, label: 'Workers' },
    { to: '/attendance', icon: Calendar, label: 'Attendance' },
    { to: '/materials', icon: Package, label: 'Materials' },
    { to: '/invoices', icon: FileText, label: 'Invoices' },
    { to: '/subcontractors', icon: Handshake, label: 'Subcontractors' },
    { to: '/notifications', icon: Bell, label: 'Notifications' },
    { to: '/settings', icon: Settings, label: 'Settings' },
  ];

  return (
    <aside className="app-sidebar">
      <div className="sidebar-brand">
        <div className="brand-logo">
          <div className="logo-square" style={{ backgroundColor: 'var(--primary)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <DraftingCompass size={20} color="white" />
          </div>
          <span style={{ color: 'var(--primary)', letterSpacing: '-0.02em', fontWeight: 800 }}>MyBrick</span>
        </div>
      </div>

      <nav className="sidebar-nav">
        {navItems.map((item) => (
          <NavLink 
            key={item.to} 
            to={item.to} 
            end={item.to === '/projects' ? false : true}
            className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
          >
            <item.icon className="nav-icon" size={20} />
            <span className="nav-label">{item.label}</span>
          </NavLink>
        ))}
      </nav>

      <div className="sidebar-footer">
        <div className="user-info">
          <div className="user-avatar">
            {user?.role?.charAt(0).toUpperCase() || 'U'}
          </div>
          <div className="user-details">
            <p className="user-name">{user?.username || 'User'}</p>
            <p className="user-role">{user?.role || 'Member'}</p>
          </div>
          <button onClick={logout} className="logout-btn" title="Logout">
            <LogOut size={18} />
          </button>
        </div>
      </div>
    </aside>
  );
};

export default Sidebar;
