-- Creazione database Spedizioni
CREATE DATABASE Spedizioni;
GO

-- Selezione del Database
USE Spedizioni;
GO

-- Creazione tabella Clienti
CREATE TABLE Clienti (
    IDCliente INT IDENTITY NOT NULL PRIMARY KEY,
    Nome NVARCHAR(50),
	Cognome NVARCHAR(50),
	NomeAzienda NVARCHAR(100),
    Indirizzo NVARCHAR(255) NOT NULL,
    CodiceFiscale NVARCHAR(16),
    PartitaIVA NVARCHAR(20),
    TipoCliente NVARCHAR(10) CHECK (TipoCliente IN ('Privato', 'Azienda')) NOT NULL,
    Email NVARCHAR(255),
    Telefono NVARCHAR(20),
    CONSTRAINT UQ_CodiceFiscale UNIQUE (CodiceFiscale),
    CONSTRAINT UQ_PartitaIVA UNIQUE (PartitaIVA)
);
GO

-- Creazione tabella Spedizioni
CREATE TABLE Spedizioni (
    IDSpedizione INT IDENTITY NOT NULL PRIMARY KEY,
    FK_IDCliente INT NOT NULL,
    DataSpedizione DATE NOT NULL,
    Peso DECIMAL(10, 2) NOT NULL,
    CittaDest NVARCHAR(255) NOT NULL,
    IndirizzoDest NVARCHAR(255) NOT NULL,
    NominativoDest NVARCHAR(255) NOT NULL,
    CostoSpedizione DECIMAL(10, 2),
    DataConsegna DATE,
    FOREIGN KEY (FK_IDCliente) REFERENCES Clienti(IDCliente)
);
GO

-- Creazione tabella AggiornamentiSpedizioni
CREATE TABLE AggiornamentiSpedizioni (
    IDAggiornamento INT IDENTITY NOT NULL PRIMARY KEY,
    FK_IDSpedizione INT NOT NULL,
    StatoSped NVARCHAR(50) CHECK (StatoSped IN ('In Transito', 'In Consegna', 'Consegnato', 'Non Consegnato')) NOT NULL,
    LuogoPacco NVARCHAR(255),
    DescrizEvento NVARCHAR(255),
    UltimoAggiornamento DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (FK_IDSpedizione) REFERENCES Spedizioni(IDSpedizione)
);

