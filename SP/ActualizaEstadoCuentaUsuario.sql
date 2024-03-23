SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fernando Nicolás
-- Create date: 2024-03-23
-- Description:	Actualiza estado de cuenta del usuario.
-- =====================================================
CREATE PROCEDURE ActualizaEstadoCuentaUsuario
AS
BEGIN
	SET NOCOUNT ON;

    UPDATE AspNetUsers
	SET EstadoCuenta = 'True'
	FROM Facturas F
	INNER JOIN AspNetUsers A ON A.Id = F.UsuarioId
	WHERE F.Pago = 'False' AND F.FechaLimitePago < GETDATE()
END
GO
