-- Create database (choose an appropriate name)
CREATE DATABASE BidSphereDB;
GO

USE BidSphereDB;
GO

-- Drop existing tables if they exist (for clean setup)
IF OBJECT_ID('Bid', 'U') IS NOT NULL
    DROP TABLE Bid;

IF OBJECT_ID('Auction', 'U') IS NOT NULL
    DROP TABLE Auction;

IF OBJECT_ID('Product', 'U') IS NOT NULL
    DROP TABLE Product;
GO

-- Create Product table
CREATE TABLE Product (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL
);
GO

-- Create Auction table
CREATE TABLE Auction (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    StartingPrice DECIMAL(18,2) NOT NULL,
    CurrentPrice DECIMAL(18,2) NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    BidCount INT NOT NULL DEFAULT(0),
    WinnerUserId NVARCHAR(450) NULL,
    RunnerUpUserId NVARCHAR(450) NULL,
    Status NVARCHAR(20) NOT NULL
);
GO

-- Create Bid table (WITHOUT the unique constraint that was causing issues)
CREATE TABLE Bid (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AuctionId INT NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    BidTime DATETIME NOT NULL
);
GO

-- Add foreign key constraints
ALTER TABLE Auction
ADD CONSTRAINT FK_Auction_Product
FOREIGN KEY (ProductId) REFERENCES Product(Id)
ON DELETE CASCADE;
GO

ALTER TABLE Bid
ADD CONSTRAINT FK_Bid_Auction
FOREIGN KEY (AuctionId) REFERENCES Auction(Id)
ON DELETE CASCADE;
GO


-- Insert sample data
INSERT INTO Product (Name, Category, Description) VALUES
('Dior Sauvage', 'Perfumes', 'Fresh and powerful fragrance'),
('Dyson Vacuum V11', 'Home Appliances', 'Strong suction cordless vacuum'),
('Honda Civic 2020 Model', 'Automobile', 'Used car in a great condition'),
('Zara Denim Jacket', 'Fashion Outfit', 'Premium blue denim jacket'),
('MacBook Pro 2023', 'Laptops', 'M2 Pro model, excellent performance'),
('Rolex Submariner', 'Luxury Watches', 'Classic luxury dive watch');
GO

INSERT INTO Auction (ProductId, StartingPrice, CurrentPrice, StartTime, EndTime, BidCount, WinnerUserId, RunnerUpUserId, Status)
VALUES
(1, 5000, 5000, DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,1,GETDATE()), 0, NULL, NULL, 'active'),
(2, 15000, 15000, GETDATE(), DATEADD(DAY,2,GETDATE()), 0, NULL, NULL, 'active'),
(3, 2000000, 2000000, DATEADD(DAY,-2,GETDATE()), DATEADD(DAY,3,GETDATE()), 0, NULL, NULL, 'active'),
(4, 3500, 3500, GETDATE(), DATEADD(DAY,5,GETDATE()), 0, NULL, NULL, 'active'),
(5, 250000, 250000, DATEADD(HOUR,-5,GETDATE()), DATEADD(DAY,1,GETDATE()), 0, NULL, NULL, 'active'),
(6, 1200000, 1200000, GETDATE(), DATEADD(DAY,4,GETDATE()), 0, NULL, NULL, 'active');
GO

-- Display confirmation message
PRINT 'Database and tables created successfully!';
PRINT 'Tables created: Product, Auction, Bid';
PRINT 'Sample data inserted.';
GO