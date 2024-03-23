SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Fernando Nicolás
-- Create date: 2024-03-22
-- Description:	Genera factura del mes por uso 
--				del API y guarda las facturas en las tablas
-- ==========================================================
CREATE PROCEDURE CreacionFacturas 
	@FechaInicio DATETIME2,
	@FechaFin DATETIME2
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @MontoPeticion DECIMAL(4, 4) = 4.0/15 -- 40 MXM pesos por cada 15 peticiones

	INSERT INTO Facturas(UsuarioId, Pago, Monto, FechaEmision, FechaLimitePago)
	SELECT L.UsuarioId, 
		   0 AS Pagada,
		   COUNT(*) * @MontoPeticion AS Monto,
		   GETDATE() AS FechaEmision,
		   DATEADD(D, 60, GETDATE()) AS FechaLimitePago
	FROM PeticionesAPI P
	INNER JOIN LlavesAPI L ON L.Id = P.LlaveId
	WHERE L.TipoLlave != 1 AND P.FechaPeticion >= @FechaInicio AND P.FechaPeticion < @FechaFin
	GROUP BY L.UsuarioId

	INSERT INTO FacturasEmitidas(Mes, Anio)
	SELECT 
		CASE MONTH(GETDATE())
			WHEN 1 THEN 12
			ELSE MONTH(GETDATE()) - 1
		END AS Mes,
		CASE MONTH(GETDATE())
			WHEN 1 THEN YEAR(GETDATE()) - 1 
			ELSE YEAR(GETDATE())
		END AS Anio
	END
GO
