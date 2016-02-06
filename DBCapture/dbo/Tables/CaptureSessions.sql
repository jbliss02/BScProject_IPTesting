CREATE TABLE [dbo].[CaptureSessions]
(
	[Id] INT IDENTITY(1,1),
	cameraId int FOREIGN KEY REFERENCES dbo.ipCameras(id),
	sessionId VARCHAR(100) PRIMARY KEY
)
