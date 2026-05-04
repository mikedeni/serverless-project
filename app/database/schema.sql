-- MySQL dump 10.13  Distrib 8.0.45, for Linux (x86_64)
--
-- Host: localhost    Database: ConstructionSaaS
-- ------------------------------------------------------
-- Server version	8.0.45-0ubuntu0.24.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Attendances`
--

DROP TABLE IF EXISTS `Attendances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Attendances` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `WorkerId` int NOT NULL,
  `ProjectId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `Date` date NOT NULL,
  `CheckIn` time DEFAULT NULL,
  `CheckOut` time DEFAULT NULL,
  `OTHours` decimal(4,1) DEFAULT '0.0',
  `Note` text,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_attendance` (`WorkerId`,`ProjectId`,`Date`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Attendances_ibfk_1` FOREIGN KEY (`WorkerId`) REFERENCES `Workers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Attendances_ibfk_2` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Attendances_ibfk_3` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Companies`
--

DROP TABLE IF EXISTS `Companies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Companies` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `DailyReportPhotos`
--

DROP TABLE IF EXISTS `DailyReportPhotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DailyReportPhotos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DailyReportId` int NOT NULL,
  `ImageUrl` varchar(1000) NOT NULL,
  `Caption` varchar(255) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `DailyReportId` (`DailyReportId`),
  CONSTRAINT `DailyReportPhotos_ibfk_1` FOREIGN KEY (`DailyReportId`) REFERENCES `DailyReports` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `DailyReports`
--

DROP TABLE IF EXISTS `DailyReports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DailyReports` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProjectId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `ReportDate` date NOT NULL,
  `Weather` enum('sunny','cloudy','rainy','stormy') DEFAULT 'sunny',
  `WorkerCount` int DEFAULT '0',
  `Summary` text NOT NULL,
  `Issues` text,
  `CreatedByUserId` int DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_daily_report` (`ProjectId`,`ReportDate`),
  KEY `CompanyId` (`CompanyId`),
  KEY `CreatedByUserId` (`CreatedByUserId`),
  CONSTRAINT `DailyReports_ibfk_1` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `DailyReports_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `DailyReports_ibfk_3` FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Documents`
--

DROP TABLE IF EXISTS `Documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Documents` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProjectId` int DEFAULT NULL,
  `CompanyId` int NOT NULL,
  `FileName` varchar(255) NOT NULL,
  `FileUrl` varchar(1000) NOT NULL,
  `FileSize` int DEFAULT NULL,
  `Category` enum('contract','blueprint','permit','receipt','report','other') DEFAULT 'other',
  `UploadedByUserId` int DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  KEY `UploadedByUserId` (`UploadedByUserId`),
  CONSTRAINT `Documents_ibfk_1` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `Documents_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Documents_ibfk_3` FOREIGN KEY (`UploadedByUserId`) REFERENCES `Users` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Expenses`
--

DROP TABLE IF EXISTS `Expenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Expenses` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProjectId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `Amount` decimal(15,2) NOT NULL,
  `Category` enum('material_cost','labor_cost','other_cost') NOT NULL,
  `Date` date NOT NULL,
  `Note` text,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Expenses_ibfk_1` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Expenses_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Invoices`
--

DROP TABLE IF EXISTS `Invoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Invoices` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProjectId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `InvoiceNumber` varchar(50) NOT NULL,
  `ClientName` varchar(255) NOT NULL,
  `Description` text,
  `Amount` decimal(15,2) NOT NULL,
  `TaxPercent` decimal(5,2) DEFAULT '7.00',
  `TaxAmount` decimal(15,2) DEFAULT '0.00',
  `TotalAmount` decimal(15,2) NOT NULL,
  `DueDate` date DEFAULT NULL,
  `Status` enum('draft','sent','paid','overdue','cancelled') DEFAULT 'draft',
  `MilestoneLabel` varchar(255) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Invoices_ibfk_1` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Invoices_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `MaterialTransactions`
--

DROP TABLE IF EXISTS `MaterialTransactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MaterialTransactions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MaterialId` int NOT NULL,
  `ProjectId` int DEFAULT NULL,
  `CompanyId` int NOT NULL,
  `Type` enum('purchase_in','requisition_out','return','adjustment') NOT NULL,
  `Qty` decimal(15,2) NOT NULL,
  `UnitPrice` decimal(15,2) DEFAULT '0.00',
  `Note` text,
  `Date` date NOT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `MaterialId` (`MaterialId`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `MaterialTransactions_ibfk_1` FOREIGN KEY (`MaterialId`) REFERENCES `Materials` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `MaterialTransactions_ibfk_2` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `MaterialTransactions_ibfk_3` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Materials`
--

DROP TABLE IF EXISTS `Materials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Materials` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompanyId` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Unit` varchar(50) NOT NULL,
  `CurrentStock` decimal(15,2) DEFAULT '0.00',
  `MinStock` decimal(15,2) DEFAULT '0.00',
  `LastPrice` decimal(15,2) DEFAULT '0.00',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `ImageUrl` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Materials_ibfk_1` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Notifications`
--

DROP TABLE IF EXISTS `Notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Notifications` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `Type` enum('task_overdue','budget_warning','low_stock','invoice_overdue','general') NOT NULL,
  `Title` varchar(255) NOT NULL,
  `Message` text NOT NULL,
  `RelatedUrl` varchar(500) DEFAULT NULL,
  `IsRead` tinyint(1) DEFAULT '0',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Notifications_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Notifications_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Payments`
--

DROP TABLE IF EXISTS `Payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Payments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `InvoiceId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `Amount` decimal(15,2) NOT NULL,
  `PaymentDate` date NOT NULL,
  `Method` enum('cash','transfer','cheque','other') DEFAULT 'transfer',
  `Note` text,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `InvoiceId` (`InvoiceId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Payments_ibfk_1` FOREIGN KEY (`InvoiceId`) REFERENCES `Invoices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Payments_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Projects`
--

DROP TABLE IF EXISTS `Projects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Projects` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompanyId` int NOT NULL,
  `ProjectName` varchar(255) NOT NULL,
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `Budget` decimal(15,2) NOT NULL DEFAULT '0.00',
  `Status` enum('planning','active','completed','on_hold') DEFAULT 'planning',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `ImageUrl` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Projects_ibfk_1` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `QuotationItems`
--

DROP TABLE IF EXISTS `QuotationItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `QuotationItems` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `QuotationId` int NOT NULL,
  `ItemOrder` int DEFAULT '0',
  `Description` varchar(500) NOT NULL,
  `Qty` decimal(15,2) NOT NULL,
  `Unit` varchar(50) NOT NULL,
  `UnitPrice` decimal(15,2) NOT NULL,
  `Amount` decimal(15,2) GENERATED ALWAYS AS ((`Qty` * `UnitPrice`)) STORED,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `QuotationId` (`QuotationId`),
  CONSTRAINT `QuotationItems_ibfk_1` FOREIGN KEY (`QuotationId`) REFERENCES `Quotations` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Quotations`
--

DROP TABLE IF EXISTS `Quotations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Quotations` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProjectId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `QuotationNumber` varchar(50) NOT NULL,
  `ClientName` varchar(255) NOT NULL,
  `ClientAddress` text,
  `ClientPhone` varchar(50) DEFAULT NULL,
  `Status` enum('draft','sent','approved','rejected') DEFAULT 'draft',
  `MarkupPercent` decimal(5,2) DEFAULT '0.00',
  `Discount` decimal(15,2) DEFAULT '0.00',
  `TaxPercent` decimal(5,2) DEFAULT '7.00',
  `Note` text,
  `ValidUntil` date DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Quotations_ibfk_1` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Quotations_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `SubcontractorContracts`
--

DROP TABLE IF EXISTS `SubcontractorContracts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SubcontractorContracts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SubcontractorId` int NOT NULL,
  `ProjectId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `Scope` text NOT NULL,
  `ContractAmount` decimal(15,2) NOT NULL,
  `PaidAmount` decimal(15,2) DEFAULT '0.00',
  `Status` enum('draft','active','completed','cancelled') DEFAULT 'draft',
  `StartDate` date DEFAULT NULL,
  `EndDate` date DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `SubcontractorId` (`SubcontractorId`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `SubcontractorContracts_ibfk_1` FOREIGN KEY (`SubcontractorId`) REFERENCES `Subcontractors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `SubcontractorContracts_ibfk_2` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `SubcontractorContracts_ibfk_3` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `SubcontractorPayments`
--

DROP TABLE IF EXISTS `SubcontractorPayments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SubcontractorPayments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ContractId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `Amount` decimal(15,2) NOT NULL,
  `PaymentDate` date NOT NULL,
  `Note` text,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `ContractId` (`ContractId`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `SubcontractorPayments_ibfk_1` FOREIGN KEY (`ContractId`) REFERENCES `SubcontractorContracts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `SubcontractorPayments_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Subcontractors`
--

DROP TABLE IF EXISTS `Subcontractors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Subcontractors` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompanyId` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Specialty` varchar(255) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Status` enum('active','inactive') DEFAULT 'active',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Subcontractors_ibfk_1` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `TaskUpdates`
--

DROP TABLE IF EXISTS `TaskUpdates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TaskUpdates` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TaskId` int NOT NULL,
  `Note` text NOT NULL,
  `ImageUrl` varchar(1000) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `TaskId` (`TaskId`),
  CONSTRAINT `TaskUpdates_ibfk_1` FOREIGN KEY (`TaskId`) REFERENCES `Tasks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Tasks`
--

DROP TABLE IF EXISTS `Tasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Tasks` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProjectId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `AssignedUserId` int DEFAULT NULL,
  `Title` varchar(255) NOT NULL,
  `Status` enum('pending','in_progress','done') DEFAULT 'pending',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `ProjectId` (`ProjectId`),
  KEY `CompanyId` (`CompanyId`),
  KEY `AssignedUserId` (`AssignedUserId`),
  CONSTRAINT `Tasks_ibfk_1` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Tasks_ibfk_2` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Tasks_ibfk_3` FOREIGN KEY (`AssignedUserId`) REFERENCES `Users` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompanyId` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Role` enum('admin','staff','foreman','viewer') DEFAULT 'staff',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Users_ibfk_1` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Workers`
--

DROP TABLE IF EXISTS `Workers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Workers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompanyId` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Position` varchar(100) DEFAULT NULL,
  `DailyWage` decimal(15,2) NOT NULL DEFAULT '0.00',
  `Phone` varchar(50) DEFAULT NULL,
  `Status` enum('active','inactive') DEFAULT 'active',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `ImageUrl` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CompanyId` (`CompanyId`),
  CONSTRAINT `Workers_ibfk_1` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-04 15:28:02
