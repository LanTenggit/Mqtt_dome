Module Function_set
    Public Ver As String = "v1.0"

    ''' <summary>
    ''' 获取应用程序的安装目录
    ''' </summary>
    ''' <remarks></remarks>
    Public progpath As String = IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) ' My.Application.Info.DirectoryPath

    ''' <summary>
    ''' 配置文件路径
    ''' </summary>
    ''' <remarks></remarks>
    Public Configurationpath As String = progpath & "\Parameter.ini"

#Region "ini读取函数"

    ''' <summary>
    ''' 声明INI配置文件读写API函数
    ''' </summary>
    ''' <param name="lpApplicationName"></param>
    ''' <param name="lpKeyName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="lpReturnedString"></param>
    ''' <param name="nSize"></param>
    ''' <param name="lpFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Int32, ByVal lpFileName As String) As Int32
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Int32
    ''' <summary>
    ''' 定义读取配置文件函数
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="AppName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As String
        Dim Str As String = LSet(Str, 256)
        GetPrivateProfileString(Section, AppName, lpDefault, Str, Len(Str), FileName)
        Return Microsoft.VisualBasic.Left(Str, InStr(Str, Chr(0)) - 1)
    End Function
    ''' <summary>
    ''' 定义写入配置文件函数
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="AppName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As Long
        WriteINI = WritePrivateProfileString(Section, AppName, lpDefault, FileName)
    End Function

#End Region

    ''' <summary>
    ''' 服务器
    ''' </summary>
    ''' <returns></returns>
    Public Property The_server As String
        Get
            Return GetINI("Configuration", "The_server", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "The_server", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 端口
    ''' </summary>
    ''' <returns></returns>
    Public Property port As String
        Get
            Return GetINI("Configuration", "port", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "port", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 客户端ID
    ''' </summary>
    ''' <returns></returns>
    Public Property client_ID As String
        Get
            Return EncryptMod.DeText(GetINI("Configuration", "client_ID", "", Configurationpath))
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "client_ID", EncryptMod.EnText(value.Trim), Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 账户
    ''' </summary>
    ''' <returns></returns>
    Public Property account As String
        Get
            Return EncryptMod.DeText(GetINI("Configuration", "account", "", Configurationpath))
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "account", EncryptMod.EnText(value.Trim), Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 密码
    ''' </summary>
    ''' <returns></returns>
    Public Property Password As String
        Get
            Return EncryptMod.DeText(GetINI("Configuration", "Password", "", Configurationpath))
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Password", EncryptMod.EnText(value.Trim), Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <returns></returns>
    Public Property Subscription_theme As String
        Get
            Return GetINI("Configuration", "Subscription_theme", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Subscription_theme", value.Trim, Configurationpath)
        End Set
    End Property

End Module
