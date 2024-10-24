-- Creación de la base de datos DataBank
BEGIN TRY
    IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'DataBank')
    BEGIN
        CREATE DATABASE DataBank;
        PRINT 'Base de datos DataBank creada correctamente.';
    END
    ELSE
    BEGIN
        PRINT 'La base de datos DataBank ya existe.';
    END
END TRY
BEGIN CATCH
    PRINT 'Ocurrió un error al crear la base de datos: ' + ERROR_MESSAGE();
END CATCH;

-- Seleccionar la base de datos DataBank y crear las tablas
BEGIN TRY
    -- Seleccionar la base de datos DataBank
    USE DataBank;

    -- Iniciamos la transacción para crear tablas
    BEGIN TRANSACTION;

    -- Creación de la tabla Persona
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Persona' AND xtype='U')
    BEGIN
        CREATE TABLE Persona (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Nombre NVARCHAR(100) NOT NULL,
            Genero NVARCHAR(50),
            Edad INT,
            Identificacion NVARCHAR(100),
            Direccion NVARCHAR(250),
            Telefono NVARCHAR(20)
        );
        PRINT 'Tabla Persona creada.';
    END

    -- Creación de la tabla Cliente
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cliente' AND xtype='U')
    BEGIN
        CREATE TABLE Cliente (
            ClienteId INT IDENTITY(1,1) PRIMARY KEY,
            PersonaId INT NOT NULL,
            Contrasena NVARCHAR(100) NOT NULL,
            Estado BIT NOT NULL,
            FOREIGN KEY (PersonaId) REFERENCES Persona(Id)
        );
        PRINT 'Tabla Cliente creada.';
    END
    
 -- Creación de la tabla Cuenta
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cuenta' AND xtype='U')
BEGIN
    CREATE TABLE Cuenta (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NumeroCuenta INT NOT NULL,
        TipoCuenta INT NOT NULL,  
        SaldoInicial DECIMAL(18, 2) NOT NULL,
        Estado BIT NOT NULL,
        ClienteId INT NOT NULL,
        CONSTRAINT UQ_NumeroCuenta UNIQUE (NumeroCuenta), -- Para garantizar que NumeroCuenta es único
        FOREIGN KEY (ClienteId) REFERENCES Cliente(ClienteId)
    );
    PRINT 'Tabla Cuenta creada.';
END



    -- Creación de la tabla Movimiento
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Movimiento' AND xtype='U')
    BEGIN
        CREATE TABLE Movimiento (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Fecha DATETIME NOT NULL,
            TipoMovimiento INT NOT NULL,
            Valor DECIMAL(18, 2) NOT NULL,
            Saldo DECIMAL(18, 2) NOT NULL,
            NumeroCuenta INT NOT NULL,
            FOREIGN KEY (NumeroCuenta) REFERENCES Cuenta(NumeroCuenta)
        );
        PRINT 'Tabla Movimiento creada.';
    END

    -- Confirmar transacción si todo es exitoso
    COMMIT TRANSACTION;
    PRINT 'Todas las tablas se han creado exitosamente en la base de datos DataBank.';

END TRY
BEGIN CATCH
    -- En caso de error, rollback
    ROLLBACK TRANSACTION;
    
    -- Mostrar el mensaje de error
    DECLARE @ErrorMessage NVARCHAR(4000);
    SET @ErrorMessage = ERROR_MESSAGE();
    PRINT 'Ocurrió un error durante la creación de las tablas: ' + @ErrorMessage;
END CATCH;
