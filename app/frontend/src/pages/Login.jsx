import React, { useState, useContext } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import { Link } from 'react-router-dom';
import './Login.css';

const Login = () => {
  const { login } = useContext(AuthContext);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    const res = await login(email, password);
    if (!res.success) {
        setError(res.error);
    }
    setIsLoading(false);
  };

  return (
    <div className="login-page-wrapper">
      <div className="bg-background font-body-md text-on-surface min-h-screen flex flex-col items-center justify-center relative overflow-hidden">
        {/* Background Blueprint Layer */}
        <div className="absolute inset-0 blueprint-bg" style={{ zIndex: 0 }}></div>
        
        {/* Hero Construction Image (Faded Overlay) */}
        <div className="absolute inset-0 opacity-10" style={{ zIndex: 1 }}>
            <img alt="Construction Site Background" className="w-full h-full object-cover" src="https://lh3.googleusercontent.com/aida-public/AB6AXuAZuw7ohxLIz1NjOMw2LXWtae11ugLeMsydnjCoh3PNXIK-bWmR2g_l7QTlsf2W7iLJ4Gt5p4uJWXV6PdxYK6t1E56uDbhKwcOGISfRa-8AVYbEG10BdtT4YXRlXQfn_yG-ZtCaXXp0CD4FRivMAw8ub1SKtRTmCD0KxB-dfZYcLYR4c8Uplz5jAB9z0BazeDbgddpWxsMopeYq8dknzkdc6v-OaNmiQWCwM3S_HJQ-1lGtNGXSPo5xT4bxGPbXmgmWVYxeU5auigjN"/>
        </div>
        
        {/* Main Container */}
        <main className="w-full max-w-[1200px] grid grid-cols-1 md:grid-cols-12 gap-gutter px-container-padding relative" style={{ zIndex: 10 }}>
            {/* Left Side: Branding & Welcome (Visible on Desktop) */}
            <div className="hidden md:flex md:col-span-7 flex-col justify-center space-y-stack-lg">
                <div className="flex items-center gap-stack-sm">
                    <span className="material-symbols-outlined text-[48px] text-primary" style={{ fontVariationSettings: "'FILL' 1" }}>construction</span>
                    <h1 className="font-h1 text-h1 text-primary">MyBrick</h1>
                </div>
                <div className="space-y-stack-md">
                    <h2 className="font-h2 text-h2 text-tertiary">แพลตฟอร์มบริหารงานก่อสร้างครบวงจร</h2>
                    <p className="font-body-lg text-body-lg text-on-surface-variant max-w-[480px]">
                        ยกระดับการบริหารงานก่อสร้างให้เป็นเรื่องง่าย ด้วยระบบ MyBrick จัดการได้ทุกมิติ ตั้งแต่ประเมินราคาจนถึงส่งมอบโครงการ
                    </p>
                </div>
                {/* Quick Feature Highlight */}
                <div className="grid grid-cols-2 gap-stack-md pt-stack-lg">
                    <div className="flex items-center gap-stack-sm text-on-tertiary-fixed-variant">
                        <span className="material-symbols-outlined">dashboard</span>
                        <span className="font-label-caps text-label-caps uppercase">DASHBOARD CONTROL</span>
                    </div>
                    <div className="flex items-center gap-stack-sm text-on-tertiary-fixed-variant">
                        <span className="material-symbols-outlined">inventory_2</span>
                        <span className="font-label-caps text-label-caps uppercase">LOGISTICS SYNC</span>
                    </div>
                </div>
            </div>
            
            {/* Right Side: Login Form Card */}
            <div className="col-span-1 md:col-span-5 flex items-center justify-center">
                <section className="w-full bg-surface-container-lowest p-stack-lg rounded-xl shadow-[0px_12px_32px_rgba(15,76,129,0.12)] border border-outline-variant">
                    {/* Logo for Mobile */}
                    <div className="flex flex-col items-center md:hidden mb-stack-lg space-y-stack-sm">
                        <div className="flex items-center gap-stack-sm">
                            <span className="material-symbols-outlined text-[32px] text-primary" style={{ fontVariationSettings: "'FILL' 1" }}>construction</span>
                            <h1 className="font-h3 text-h3 text-primary">MyBrick</h1>
                        </div>
                        <p className="font-body-sm text-body-sm text-on-surface-variant">เข้าสู่ระบบจัดการโครงการ</p>
                    </div>
                    
                    <div className="mb-stack-lg">
                        <h3 className="font-h3 text-h3 text-on-surface">Login</h3>
                        <p className="font-body-sm text-body-sm text-on-surface-variant">ระบุข้อมูลของคุณเพื่อเข้าใช้งานระบบ</p>
                    </div>
                    
                    {error && (
                        <div className="mb-stack-lg p-stack-sm rounded-lg bg-error-container text-on-error-container border border-error/20 font-body-sm">
                            {error}
                        </div>
                    )}
                    
                    {/* Form */}
                    <form className="space-y-stack-md" onSubmit={handleSubmit}>
                        {/* Email Field */}
                        <div className="space-y-unit">
                            <label className="font-label-caps text-label-caps text-on-surface-variant" htmlFor="email">EMAIL ADDRESS</label>
                            <div className="relative group">
                                <span className="material-symbols-outlined absolute left-stack-md top-1/2 -translate-y-1/2 text-outline group-focus-within:text-secondary transition-colors">mail</span>
                                <input 
                                    className="w-full pl-[48px] pr-stack-md py-stack-md bg-surface border border-outline-variant rounded-lg font-body-md focus:outline-none focus:ring-2 focus:ring-secondary/20 focus:border-secondary transition-all" 
                                    id="email" 
                                    placeholder="name@company.com" 
                                    type="email"
                                    required
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                />
                            </div>
                        </div>
                        {/* Password Field */}
                        <div className="space-y-unit">
                            <label className="font-label-caps text-label-caps text-on-surface-variant" htmlFor="password">PASSWORD</label>
                            <div className="relative group">
                                <span className="material-symbols-outlined absolute left-stack-md top-1/2 -translate-y-1/2 text-outline group-focus-within:text-secondary transition-colors">lock</span>
                                <input 
                                    className="w-full pl-[48px] pr-stack-md py-stack-md bg-surface border border-outline-variant rounded-lg font-body-md focus:outline-none focus:ring-2 focus:ring-secondary/20 focus:border-secondary transition-all" 
                                    id="password" 
                                    placeholder="••••••••" 
                                    type="password"
                                    required
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                />
                            </div>
                        </div>
                        {/* Options Row */}
                        <div className="flex items-center justify-between pt-stack-sm">
                            <label className="flex items-center gap-stack-sm cursor-pointer group">
                                <input className="w-4 h-4 rounded border-outline-variant text-secondary focus:ring-secondary" type="checkbox"/>
                                <span className="font-body-sm text-body-sm text-on-surface-variant group-hover:text-on-surface">จดจำฉัน</span>
                            </label>
                            <a className="font-body-sm text-body-sm text-secondary hover:underline" href="#">ลืมรหัสผ่าน?</a>
                        </div>
                        {/* Submit Button */}
                        <button 
                            disabled={isLoading}
                            className="w-full bg-primary hover:bg-secondary text-on-primary py-stack-md rounded-lg font-h3 transition-all flex items-center justify-center gap-stack-sm active:scale-[0.98] disabled:opacity-70 mt-stack-md" 
                            type="submit"
                        >
                            <span>{isLoading ? 'Signing in...' : 'Login'}</span>
                            {!isLoading && <span className="material-symbols-outlined">arrow_forward</span>}
                        </button>
                    </form>
                    
                    {/* Footer Support */}
                    <div className="mt-stack-lg pt-stack-md border-t border-surface-container-high text-center">
                        <p className="font-body-sm text-body-sm text-on-surface-variant">
                            ยังไม่มีบัญชีผู้ใช้งาน? <Link to="/register" className="text-primary font-semibold hover:underline">สมัครใช้งานระบบ</Link>
                        </p>
                    </div>
                </section>
            </div>
        </main>
        
        {/* System Info Footer (Floating) */}
        <footer className="absolute bottom-stack-md w-full text-center z-10 pointer-events-none">
            <p className="font-label-caps text-label-caps text-on-tertiary-fixed-variant opacity-60">
                © 2024 MYBRICK ENTERPRISE SUITE. ALL RIGHTS RESERVED.
            </p>
        </footer>
      </div>
    </div>
  );
};

export default Login;
