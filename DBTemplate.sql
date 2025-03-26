------------------------------------------------------------------------------------
-- OrderServiceDB
------------------------------------------------------------------------------------
USE master
GO
CREATE DATABASE OrderServiceDB
GO

USE OrderServiceDB
GO

CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT,
    ProductId INT,
    Quantity INT,
    TotalAmount DECIMAL(18, 2),
    OrderDate DATETIME
);
GO

------------------------------------------------------------------------------------
-- ProductServiceDB
------------------------------------------------------------------------------------
USE master
GO
CREATE DATABASE ProductServiceDB
GO

USE ProductServiceDB
GO

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255),
    Price DECIMAL(18, 2),
	Stock INT
);
GO

------------------------------------------------------------------------------------
-- ProductServiceDB
------------------------------------------------------------------------------------
USE master
GO
CREATE DATABASE CustomerServiceDB
GO

USE CustomerServiceDB
GO

CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(255),
    LastName VARCHAR(255),
    Email VARCHAR(255)
);
GO