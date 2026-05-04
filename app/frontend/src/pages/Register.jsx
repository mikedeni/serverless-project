import React, { useState, useContext } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import { Link } from 'react-router-dom';
import { 
    Construction, 
    User, 
    Building2, 
    Mail, 
    Lock, 
    ArrowRight,
    CheckCircle2
} from 'lucide-react';

const Register = () => {
  const { register } = useContext(AuthContext);
  const [formData, setFormData] = useState({
      name: '',
      companyName: '',
      email: '',
      password: ''
  });
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    const res = await register(formData.name, formData.companyName, formData.email, formData.password);
    if (!res.success) {
        setError(res.error);
    }
    setIsLoading(false);
  };

  const handleChange = (e) => {
      setFormData({...formData, [e.target.id]: e.target.value });
  }

  return (
    <div className="login-page-wrapper blueprint-bg" style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <main style={{ width: '100%', maxWidth: '1000px', display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '40px', padding: '40px', position: 'relative', zIndex: 10 }}>
          
          {/* Left Side: Info */}
          <div style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', color: 'var(--on-surface)' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '32px' }}>
                  <div style={{ background: 'var(--primary)', color: 'white', padding: '12px', borderRadius: '12px' }}>
                    <Construction size={32} />
                  </div>
                  <h1 className="text-h1" style={{ color: 'var(--primary)' }}>MyBrick</h1>
              </div>
              
              <h2 className="text-h2" style={{ color: 'var(--tertiary)', marginBottom: '24px' }}>สร้างพื้นที่ทำงานของคุณ</h2>
              <p className="text-body-lg" style={{ color: 'var(--on-surface-variant)', marginBottom: '40px', maxWidth: '400px' }}>
                  เริ่มต้นจัดการโครงการก่อสร้างของคุณอย่างมืออาชีพด้วย MyBrick สมัครสมาชิกวันนี้เพื่อยกระดับการบริหารจัดการทีมและงบประมาณ
              </p>

              <div style={{ display: 'grid', gap: '16px' }}>
                  {[
                      'จัดการงบประมาณแบบ Real-time',
                      'ติดตามการเข้างานและค่าแรง',
                      'ระบบคลังวัสดุอัจฉริยะ',
                      'รายงานทางการเงินครบวงจร'
                  ].map((text, i) => (
                      <div key={i} style={{ display: 'flex', alignItems: 'center', gap: '12px', color: 'var(--on-surface-variant)' }}>
                          <CheckCircle2 size={18} color="var(--success)" />
                          <span className="text-body-md" style={{ fontWeight: 500 }}>{text}</span>
                      </div>
                  ))}
              </div>
          </div>
          
          {/* Right Side: Form */}
          <div className="glass-panel" style={{ padding: '48px', boxShadow: 'var(--shadow-premium)' }}>
              <div style={{ marginBottom: '32px' }}>
                  <h3 className="text-h3">Register</h3>
                  <p className="text-body-sm" style={{ color: 'var(--on-surface-variant)' }}>สร้างบัญชีผู้ใช้งานใหม่สำหรับองค์กรของคุณ</p>
              </div>

              {error && (
                  <div className="auth-error" style={{ marginBottom: '24px', background: 'var(--error-container)', color: 'var(--on-error-container)', padding: '12px', borderRadius: '8px', fontSize: '14px' }}>
                      {error}
                  </div>
              )}

              <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '20px' }}>
                  <div className="input-group">
                      <label className="text-label-caps">Full Name</label>
                      <div style={{ position: 'relative' }}>
                          <User size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                          <input 
                              className="input-field" 
                              style={{ paddingLeft: '40px', marginBottom: 0 }}
                              id="name" 
                              required 
                              value={formData.name} 
                              onChange={handleChange} 
                              placeholder="Your Name"
                          />
                      </div>
                  </div>

                  <div className="input-group">
                      <label className="text-label-caps">Company Name</label>
                      <div style={{ position: 'relative' }}>
                          <Building2 size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                          <input 
                              className="input-field" 
                              style={{ paddingLeft: '40px', marginBottom: 0 }}
                              id="companyName" 
                              required 
                              value={formData.companyName} 
                              onChange={handleChange} 
                              placeholder="Enterprise Name"
                          />
                      </div>
                  </div>

                  <div className="input-group">
                      <label className="text-label-caps">Work Email</label>
                      <div style={{ position: 'relative' }}>
                          <Mail size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                          <input 
                              className="input-field" 
                              style={{ paddingLeft: '40px', marginBottom: 0 }}
                              type="email" 
                              id="email" 
                              required 
                              value={formData.email} 
                              onChange={handleChange} 
                              placeholder="name@company.com"
                          />
                      </div>
                  </div>

                  <div className="input-group">
                      <label className="text-label-caps">Secure Password</label>
                      <div style={{ position: 'relative' }}>
                          <Lock size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                          <input 
                              className="input-field" 
                              style={{ paddingLeft: '40px', marginBottom: 0 }}
                              type="password" 
                              id="password" 
                              required 
                              minLength={6} 
                              value={formData.password} 
                              onChange={handleChange} 
                              placeholder="••••••••"
                          />
                      </div>
                  </div>

                  <button 
                      type="submit" 
                      disabled={isLoading} 
                      className="btn-primary" 
                      style={{ marginTop: '12px', height: '52px' }}
                  >
                      {isLoading ? <div className="spinner-small" style={{ borderTopColor: 'white' }}></div> : <><CheckCircle2 size={18} /> Create Workspace</>}
                  </button>
              </form>

              <div style={{ marginTop: '32px', textAlign: 'center', paddingTop: '24px', borderTop: '1px solid var(--outline-variant)' }}>
                  <p className="text-body-sm" style={{ color: 'var(--on-surface-variant)' }}>
                      Already have an account? <Link to="/login" style={{ color: 'var(--primary)', fontWeight: 700 }}>Sign in here</Link>
                  </p>
              </div>
          </div>
      </main>

      <footer style={{ position: 'absolute', bottom: '24px', width: '100%', textAlign: 'center', opacity: 0.5 }}>
          <p className="text-label-caps" style={{ fontSize: '10px' }}>© 2024 MYBRICK ENTERPRISE SUITE. ALL RIGHTS RESERVED.</p>
      </footer>
    </div>
  );
};

export default Register;
