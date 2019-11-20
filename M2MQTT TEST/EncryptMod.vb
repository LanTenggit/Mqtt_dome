Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Public Class EncryptMod
    '加密集合
    ''' <summary>
    ''' 用MD5加密方式加密文本,可以直接使用。在模块中的主要作用是用于加密关键字 (返回值:加密后的文本)
    ''' </summary>
    ''' <param name="strData">需要加密的文本</param>
    ''' <remarks></remarks>
    ''' 
    Private Shared Function MD5EncryptProc(ByVal strData As String) As String
        Dim MD As New System.Security.Cryptography.MD5CryptoServiceProvider
        Try
            Return System.Text.Encoding.Default.GetString(MD.ComputeHash(System.Text.Encoding.Default.GetBytes(strData.Trim)))
        Catch ex As Exception
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 加密 (返回值:加密后的文本)
    ''' </summary>
    ''' <param name="StrText">需要加密的文本</param>
    ''' <param name="strKey">加密时使用的关键字，俗称密码。</param>
    ''' <remarks></remarks>
    ''' 
    Public Shared Function EnText(ByVal StrText As String, Optional ByVal strKey As String = "13720102721") As String
        Try
            Dim Des As New DESCryptoServiceProvider
            Dim inputByteArray() As Byte

            inputByteArray = Encoding.Default.GetBytes(StrText)
            Des.Key = ASCIIEncoding.ASCII.GetBytes(Left(MD5EncryptProc(strKey), 8).PadRight(8))
            Des.IV = ASCIIEncoding.ASCII.GetBytes(Left(MD5EncryptProc(strKey), 8).PadRight(8))

            Dim MS As New System.IO.MemoryStream
            Dim CS As New CryptoStream(MS, Des.CreateEncryptor, CryptoStreamMode.Write)
            CS.Write(inputByteArray, 0, inputByteArray.Length)
            CS.FlushFinalBlock()

            Dim Ret As New StringBuilder

            For Each b As Byte In MS.ToArray()
                Ret.AppendFormat("{0:X2}", b)
            Next

            Return Ret.ToString()
        Catch ex As Exception
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 解密 (返回值:解密后的文本)
    ''' </summary>
    ''' <param name="strText">需要解密的文本</param>
    ''' <param name="strKey">解密时使用的关键字，必需和加密时的关健字相同，才可以解密出正确的文本</param>
    ''' <remarks></remarks>
    ''' 
    Public Shared Function DeText(ByVal strText As String, Optional ByVal strKey As String = "13720102721") As String
        Try
            Dim Des As New DESCryptoServiceProvider
            Dim intLen As Integer
            intLen = strText.Length / 2 - 1
            Dim inputByteArray(intLen) As Byte
            Dim x, i As Integer
            For x = 0 To intLen
                i = Convert.ToInt32(strText.Substring(x * 2, 2), 16)
                inputByteArray(x) = CType(i, Byte)
            Next

            Des.Key = ASCIIEncoding.ASCII.GetBytes(Left(MD5EncryptProc(strKey), 8).PadRight(8))
            Des.IV = ASCIIEncoding.ASCII.GetBytes(Left(MD5EncryptProc(strKey), 8).PadRight(8))

            Dim MS As New System.IO.MemoryStream

            Dim CS As New CryptoStream(MS, Des.CreateDecryptor, CryptoStreamMode.Write)

            CS.Write(inputByteArray, 0, inputByteArray.Length)

            CS.FlushFinalBlock()

            Return Encoding.Default.GetString(MS.ToArray)
        Catch ex As Exception
            '  MsgBox(ex.ToString)
            Return ""
        End Try
    End Function

End Class
