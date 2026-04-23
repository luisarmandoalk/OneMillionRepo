CREATE TABLE [dbo].[leads] (
    [id] UNIQUEIDENTIFIER NOT NULL,
    [nombre] NVARCHAR(200) NOT NULL,
    [email] NVARCHAR(320) NOT NULL,
    [telefono] NVARCHAR(50) NULL,
    [fuente] NVARCHAR(50) NOT NULL,
    [producto_interes] NVARCHAR(200) NULL,
    [presupuesto] DECIMAL(18,2) NULL,
    [created_at] DATETIME2 NOT NULL,
    [updated_at] DATETIME2 NOT NULL,
    [deleted_at] DATETIME2 NULL,
    CONSTRAINT [PK_leads] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [ix_leads_email]
ON [dbo].[leads] ([email]);
