CREATE TABLE [dbo].[Bill] (
    [BillID]     INT            IDENTITY (1, 1) NOT NULL,
    [Date]       DATETIME       NOT NULL,
    [CustomerID] INT            NOT NULL,
    [TaxRate]    DECIMAL (5, 4) NOT NULL,
    PRIMARY KEY CLUSTERED ([BillID] ASC),
    CONSTRAINT [FK_Bill_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID])
);

CREATE TABLE [dbo].[Bill_Products] (
    [BillID]    INT        NOT NULL,
    [ProductID] INT        NOT NULL,
    [Price]     FLOAT (53) NOT NULL,
    CONSTRAINT [PK_Bill_Products] PRIMARY KEY CLUSTERED ([BillID] ASC, [ProductID] ASC),
    CONSTRAINT [FK_Bill_Products_Bill] FOREIGN KEY ([BillID]) REFERENCES [dbo].[Bill] ([BillID]),
    CONSTRAINT [FK_Bill_Products_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ProductID])
);

CREATE TABLE [dbo].[BillSummaryRow] (
    [RowID]         INT             IDENTITY (1, 1) NOT NULL,
    [BillID]        INT             NOT NULL,
    [Cname]         VARCHAR (50)    NOT NULL,
    [DateOfInvoice] DATETIME        NOT NULL,
    [LineTotal]     DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([RowID] ASC)
);

CREATE TABLE [dbo].[Customer] (
    [CustomerID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (50)  NOT NULL,
    [Phone]      VARCHAR (20)  NOT NULL,
    [Email]      VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC)
);

CREATE TABLE [dbo].[Product] (
    [ProductID]       INT             IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (100)   NOT NULL,
    [Category]        INT             NOT NULL,
    [UnitPrice]       DECIMAL (18, 2) NOT NULL,
    [QuantityInStock] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductID] ASC),
    CONSTRAINT [FK_Product_Category] FOREIGN KEY ([Category]) REFERENCES [dbo].[ProductCategories] ([ProductCategories])
);

CREATE TABLE [dbo].[ProductCategories] (
    [ProductCategories] INT          NOT NULL,
    [Name]              VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductCategories] ASC)
);

CREATE TABLE [dbo].[UserLogin] (
    [UserID]       INT           IDENTITY (1, 1) NOT NULL,
    [Username]     VARCHAR (50)  NOT NULL,
    [PasswordHash] VARCHAR (255) NOT NULL,
    [Email]        VARCHAR (100) NOT NULL,
    [Phone]        VARCHAR (20)  NOT NULL,
    [CreatedAt]    DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);

ALTER TABLE Bill_Products
ADD Quantity INT NOT NULL DEFAULT 1;
