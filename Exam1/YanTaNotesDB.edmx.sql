
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/16/2022 13:09:14
-- Generated from EDMX file: C:\Users\Hamza Rehioui\source\repos\Exam1\Exam1\YanTaNotesDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [YanTaNotesDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Notes'
CREATE TABLE [dbo].[Notes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [Author] nvarchar(max)  NOT NULL,
    [Created] datetime  NOT NULL,
    [LastEdited] datetime  NOT NULL,
    [Comments] nvarchar(max)  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [userID] int  NOT NULL,
    [User_id] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(max)  NOT NULL,
    [password] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'Notes'
ALTER TABLE [dbo].[Notes]
ADD CONSTRAINT [PK_Notes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_id] in table 'Notes'
ALTER TABLE [dbo].[Notes]
ADD CONSTRAINT [FK_UserNote]
    FOREIGN KEY ([User_id])
    REFERENCES [dbo].[Users]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserNote'
CREATE INDEX [IX_FK_UserNote]
ON [dbo].[Notes]
    ([User_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------