Imports uPLibrary.Networking.M2Mqtt
Imports uPLibrary.Networking.M2Mqtt.Messages

Public Class Form1
    Implements IDisposable

    ''' <summary>
    ''' 实例化订阅客户端
    ''' </summary>
    Public Property M2Client As MqttClient
    ''' <summary>
    ''' 是否启动服务
    ''' </summary>
    Dim myBol As Boolean = False

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Select Case myBol
            Case False
                Dim brokerHostName As String = TextBox3.Text.Trim '"10.114.182.131"
                Dim brokerPort As Integer = TextBox4.Text.Trim '61613
                Dim clientId As String = TextBox5.Text.Trim '"m2mqtt" & Guid.NewGuid.ToString
                Dim username As String = TextBox6.Text.Trim ' "admin"
                Dim password As String = TextBox7.Text.Trim ' "password"
                '无SSL连接
                M2Client = New MqttClient(brokerHostName, brokerPort, False, Nothing, Nothing, MqttSslProtocols.None)
                Try
                    '消息接受
                    AddHandler M2Client.MqttMsgPublishReceived, New MqttClient.MqttMsgPublishEventHandler(AddressOf MqttMsgPublishEventHandler)
                    '取消订阅
                    AddHandler M2Client.MqttMsgUnsubscribed, New MqttClient.MqttMsgUnsubscribedEventHandler(AddressOf MqttMsgUnsubscribedEventHandler)
                    '推送信息
                    AddHandler M2Client.MqttMsgPublished, New MqttClient.MqttMsgPublishedEventHandler(AddressOf MqttMsgPublishedEventHandler)
                    '关闭连接
                    AddHandler M2Client.ConnectionClosed, New MqttClient.ConnectionClosedEventHandler(AddressOf ConnectionClosedEventHandler)

                    '连接Broker
                    M2Client.Connect(clientId, username, password)  '问题记录:当服务器网络存在出站防火墙不能通过时,m2mqtt CPU暴涨到30%,报错 目标主机积极的拒绝连接, 并以很快的速度死循环
                    TextBox1.AppendText("连接成功!" & vbCrLf)
                    Button1.Text = "关闭连接"
                    myBol = True
                Catch ex As System.Exception
                    myBol = False
                    'CType(client, IDisposable).Dispose()  '连接失败的话就断开连接
                    TextBox1.AppendText("连接失败!" & vbCrLf)
                End Try
            Case True
                myBol = False
                M2Client.Disconnect()
                Button1.Text = "连接服务器"
        End Select
    End Sub

    ''' <summary>
    ''' 订阅显示
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub MqttMsgPublishEventHandler(sender As Object, e As MqttMsgPublishEventArgs)
        Dim msg As String = "Topic:" + e.Topic + "   Message:" + System.Text.Encoding.[Default].GetString(e.Message)
        Me.Invoke(New Action(Sub()
                                 TextBox1.AppendText(msg & vbCrLf)
                             End Sub))
    End Sub

    ''' <summary>
    ''' mqtt消息取消订阅事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub MqttMsgUnsubscribedEventHandler(sender As Object, e As MqttMsgUnsubscribedEventArgs)

        System.Threading.ThreadPool.QueueUserWorkItem(New Threading.WaitCallback(Sub()

                                                                                 End Sub))

    End Sub

    ''' <summary>
    ''' MQTT消息发布事件处理程序
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub MqttMsgPublishedEventHandler(sender As Object, e As MqttMsgPublishedEventArgs)

    End Sub

    ''' <summary>
    ''' 连接已关闭事件处理程序
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub ConnectionClosedEventHandler(sender As Object, e As EventArgs)
        Try
            Me.Invoke(New Action(Sub()
                                     TextBox1.AppendText("连接被关闭" & vbCrLf)
                                 End Sub))
            If M2Client IsNot Nothing Then
                If M2Client.IsConnected = True Then
                    M2Client.Disconnect() '关闭连接
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' 推送
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If M2Client IsNot Nothing Then
            If M2Client.IsConnected = True Then
                client_MqttMsgPublish(Subscription_theme, String.Format("{2} - {0}:{1}", Format(Now, "yyyy-MM-dd HH:mm:ss"), TextBox2.Text & vbCrLf, TextBox5.Text))
            End If
        End If
    End Sub

    ''' <summary>
    ''' 推送信息
    ''' </summary>
    ''' <param name="publishString"></param>
    Public Sub client_MqttMsgPublish(ByVal topic As String, publishString As String)
        If M2Client IsNot Nothing Then
            If M2Client.IsConnected = True Then
                M2Client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(publishString), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, True)
            End If
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If M2Client IsNot Nothing Then
            If M2Client.IsConnected = True Then
                M2Client.Disconnect() '关闭连接
                M2Client = Nothing
            End If
        End If
        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill() '结束自己
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If M2Client IsNot Nothing Then
            If M2Client.IsConnected = True Then
                Dim topic As String() = {TextBox8.Text.Trim} '{"topic"} '订阅主题
                Dim qosLevels As Byte() = {MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE}
                '订阅
                M2Client.Subscribe(topic, qosLevels)
            End If
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        TextBox3.Text = The_server

        TextBox4.Text = port

        TextBox5.Text = client_ID

        TextBox6.Text = account

        TextBox7.Text = Password

        TextBox8.Text = Subscription_theme




    End Sub

    ''' <summary>
    ''' 随机客户端ID
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        client_ID = Guid.NewGuid.ToString()
        TextBox5.Text = client_ID
    End Sub

    ''' <summary>
    ''' 撤销订阅
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If M2Client IsNot Nothing Then
            If M2Client.IsConnected = True Then
                Dim topic As String() = {TextBox8.Text.Trim}
                M2Client.Unsubscribe(topic)
            End If
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim thread As New Threading.Thread(Sub()
                                               For i = 0 To 1000
                                                   Threading.Thread.Sleep(10)
                                                   If M2Client IsNot Nothing Then
                                                       client_MqttMsgPublish(Subscription_theme, String.Format("{2} - {0}:{1}",
                                                                                           Format(Now, "yyyy-MM-dd HH:mm:ss"),
                                                                                           i.ToString & vbCrLf,
                                                                                           TextBox5.Text))
                                                   Else
                                                       Exit For
                                                   End If
                                               Next
                                           End Sub) With {
                                           .IsBackground = True,
                                           .Name = "mqtt压力测试"}
        thread.Start()

    End Sub

    ''' <summary>
    ''' 服务器
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        The_server = TextBox3.Text.Trim
    End Sub

    ''' <summary>
    ''' 端口
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        port = TextBox4.Text.Trim
    End Sub

    ''' <summary>
    ''' 客户端ID
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        client_ID = TextBox5.Text.Trim
    End Sub

    ''' <summary>
    ''' 账户
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged
        account = TextBox6.Text.Trim
    End Sub

    ''' <summary>
    ''' 密码
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox7_TextChanged(sender As Object, e As EventArgs) Handles TextBox7.TextChanged
        Password = TextBox7.Text.Trim
    End Sub

    ''' <summary>
    ''' 订阅主题
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox8_TextChanged(sender As Object, e As EventArgs) Handles TextBox8.TextChanged
        Subscription_theme = TextBox8.Text.Trim
    End Sub
End Class
