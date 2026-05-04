-- =============================================
-- Phase 2: Core Business Features
-- =============================================

-- 7. Quotations (ใบเสนอราคา)
CREATE TABLE Quotations (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjectId INT NOT NULL,
    CompanyId INT NOT NULL,
    QuotationNumber VARCHAR(50) NOT NULL,
    ClientName VARCHAR(255) NOT NULL,
    ClientAddress TEXT,
    ClientPhone VARCHAR(50),
    Status ENUM('draft','sent','approved','rejected') DEFAULT 'draft',
    MarkupPercent DECIMAL(5,2) DEFAULT 0,
    Discount DECIMAL(15,2) DEFAULT 0,
    TaxPercent DECIMAL(5,2) DEFAULT 7.00,
    Note TEXT,
    ValidUntil DATE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- 8. Quotation Items (รายการ BOQ)
CREATE TABLE QuotationItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    QuotationId INT NOT NULL,
    ItemOrder INT DEFAULT 0,
    Description VARCHAR(500) NOT NULL,
    Qty DECIMAL(15,2) NOT NULL,
    Unit VARCHAR(50) NOT NULL,
    UnitPrice DECIMAL(15,2) NOT NULL,
    Amount DECIMAL(15,2) GENERATED ALWAYS AS (Qty * UnitPrice) STORED,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (QuotationId) REFERENCES Quotations(Id) ON DELETE CASCADE
);

-- 9. Workers (ทะเบียนแรงงาน)
CREATE TABLE Workers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CompanyId INT NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Position VARCHAR(100),
    DailyWage DECIMAL(15,2) NOT NULL DEFAULT 0,
    Phone VARCHAR(50),
    Status ENUM('active','inactive') DEFAULT 'active',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- 10. Attendances (ลงเวลาเข้า-ออก)
CREATE TABLE Attendances (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    WorkerId INT NOT NULL,
    ProjectId INT NOT NULL,
    CompanyId INT NOT NULL,
    Date DATE NOT NULL,
    CheckIn TIME,
    CheckOut TIME,
    OTHours DECIMAL(4,1) DEFAULT 0,
    Note TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (WorkerId) REFERENCES Workers(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_attendance (WorkerId, ProjectId, Date)
);

-- 11. Materials (คลังวัสดุกลาง)
CREATE TABLE Materials (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CompanyId INT NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Unit VARCHAR(50) NOT NULL,
    CurrentStock DECIMAL(15,2) DEFAULT 0,
    MinStock DECIMAL(15,2) DEFAULT 0,
    LastPrice DECIMAL(15,2) DEFAULT 0,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- 12. Material Transactions (การเคลื่อนไหววัสดุ)
CREATE TABLE MaterialTransactions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MaterialId INT NOT NULL,
    ProjectId INT,
    CompanyId INT NOT NULL,
    Type ENUM('purchase_in','requisition_out','return','adjustment') NOT NULL,
    Qty DECIMAL(15,2) NOT NULL,
    UnitPrice DECIMAL(15,2) DEFAULT 0,
    Note TEXT,
    Date DATE NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaterialId) REFERENCES Materials(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE SET NULL,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- =============================================
-- Phase 3: Money Flow
-- =============================================

-- 13. Invoices (ใบแจ้งหนี้)
CREATE TABLE Invoices (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjectId INT NOT NULL,
    CompanyId INT NOT NULL,
    InvoiceNumber VARCHAR(50) NOT NULL,
    ClientName VARCHAR(255) NOT NULL,
    Description TEXT,
    Amount DECIMAL(15,2) NOT NULL,
    TaxPercent DECIMAL(5,2) DEFAULT 7.00,
    TaxAmount DECIMAL(15,2) DEFAULT 0,
    TotalAmount DECIMAL(15,2) NOT NULL,
    DueDate DATE,
    Status ENUM('draft','sent','paid','overdue','cancelled') DEFAULT 'draft',
    MilestoneLabel VARCHAR(255),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- 14. Payments (การรับเงิน)
CREATE TABLE Payments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    InvoiceId INT NOT NULL,
    CompanyId INT NOT NULL,
    Amount DECIMAL(15,2) NOT NULL,
    PaymentDate DATE NOT NULL,
    Method ENUM('cash','transfer','cheque','other') DEFAULT 'transfer',
    Note TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- =============================================
-- Phase 4: Site Operations
-- =============================================

-- 15. Daily Reports (รายงานรายวัน)
CREATE TABLE DailyReports (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjectId INT NOT NULL,
    CompanyId INT NOT NULL,
    ReportDate DATE NOT NULL,
    Weather ENUM('sunny','cloudy','rainy','stormy') DEFAULT 'sunny',
    WorkerCount INT DEFAULT 0,
    Summary TEXT NOT NULL,
    Issues TEXT,
    CreatedByUserId INT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id) ON DELETE SET NULL,
    UNIQUE KEY unique_daily_report (ProjectId, ReportDate)
);

-- 16. Daily Report Photos
CREATE TABLE DailyReportPhotos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    DailyReportId INT NOT NULL,
    ImageUrl VARCHAR(1000) NOT NULL,
    Caption VARCHAR(255),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (DailyReportId) REFERENCES DailyReports(Id) ON DELETE CASCADE
);

-- 17. Subcontractors (ผู้รับเหมาช่วง)
CREATE TABLE Subcontractors (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CompanyId INT NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Specialty VARCHAR(255),
    Phone VARCHAR(50),
    Email VARCHAR(255),
    Status ENUM('active','inactive') DEFAULT 'active',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- 18. Subcontractor Contracts
CREATE TABLE SubcontractorContracts (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SubcontractorId INT NOT NULL,
    ProjectId INT NOT NULL,
    CompanyId INT NOT NULL,
    Scope TEXT NOT NULL,
    ContractAmount DECIMAL(15,2) NOT NULL,
    PaidAmount DECIMAL(15,2) DEFAULT 0,
    Status ENUM('draft','active','completed','cancelled') DEFAULT 'draft',
    StartDate DATE,
    EndDate DATE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SubcontractorId) REFERENCES Subcontractors(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- 19. Subcontractor Payments
CREATE TABLE SubcontractorPayments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ContractId INT NOT NULL,
    CompanyId INT NOT NULL,
    Amount DECIMAL(15,2) NOT NULL,
    PaymentDate DATE NOT NULL,
    Note TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ContractId) REFERENCES SubcontractorContracts(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- 20. Notifications (แจ้งเตือน)
CREATE TABLE Notifications (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CompanyId INT NOT NULL,
    Type ENUM('task_overdue','budget_warning','low_stock','invoice_overdue','general') NOT NULL,
    Title VARCHAR(255) NOT NULL,
    Message TEXT NOT NULL,
    RelatedUrl VARCHAR(500),
    IsRead TINYINT(1) DEFAULT 0,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- =============================================
-- Phase 5: Documents & Infrastructure
-- =============================================

-- 21. Documents (เอกสารโครงการ)
CREATE TABLE Documents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjectId INT,
    CompanyId INT NOT NULL,
    FileName VARCHAR(255) NOT NULL,
    FileUrl VARCHAR(1000) NOT NULL,
    FileSize INT,
    Category ENUM('contract','blueprint','permit','receipt','report','other') DEFAULT 'other',
    UploadedByUserId INT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE SET NULL,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    FOREIGN KEY (UploadedByUserId) REFERENCES Users(Id) ON DELETE SET NULL
);

