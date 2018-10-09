package pl.ejdriansoft.chatr.ui


import android.annotation.SuppressLint
import android.os.Bundle
import android.provider.Settings
import android.provider.Settings.Secure.getString
import android.widget.ArrayAdapter
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.crashlytics.android.Crashlytics
import com.google.gson.Gson
import com.smartarmenia.dotnetcoresignalrclientjava.*
import io.fabric.sdk.android.Fabric
import kotlinx.android.synthetic.main.activity_chat.*
import org.jetbrains.anko.*
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.services.ChatRAPI
import pl.ejdriansoft.chatr.services.api.Prefs
import java.util.ArrayList


class ChatActivity : AppCompatActivity(), HubConnectionListener, HubEventListener, AnkoLogger {

    lateinit var arrayAdapter: ArrayAdapter<String>
    private lateinit var hubConnection: WebSocketHubConnectionP2

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)

        hubConnection = WebSocketHubConnectionP2("http://ejdriansoft.cloud/chat", "")
        val name = Prefs(this).nickname

        val messageList = ArrayList<String>()
        arrayAdapter = ArrayAdapter(this@ChatActivity,
                R.layout.item_chat, messageList)
        lvMessages.adapter = arrayAdapter
        getMessages(arrayAdapter)

        bSend.setOnClickListener { _ ->
            val message = etMessageText.text.toString()
            etMessageText.setText("")
            try {
                hubConnection.invoke("Send", name, message)
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
        startHubConnection(hubConnection)

        hubConnection.subscribeToEvent("broadcastMessage", this)
        hubConnection.subscribeToEvent("currentConnections") { message ->
            runOnUiThread {
                tvMain.text = "Current connections: ${message.arguments[0].asString}"
            }
        }
    }

    private fun getMessages(adapter: ArrayAdapter<String>) {
        val call = ChatRAPI().allMessages()
        doAsync {
            val response = call.execute()
            if (response.isSuccessful) {
                runOnUiThread {
                    response.body()?.forEach {
                        adapter.add("${it.name}: ${it.body}")
                    }
                    adapter.notifyDataSetChanged()
                    lvMessages.smoothScrollToPosition(adapter.count)
                }
            } else {
                runOnUiThread {
                    toast("Error loading messages").show()
                }
            }
        }
    }

    private fun startHubConnection(hub: HubConnection) {
        try {
            hub.connect()
        } catch (e: Exception) {
            e.printStackTrace()
            tvMain.text = e.message
        }
    }

    override fun onEventMessage(message: HubMessage) {
        runOnUiThread {
            arrayAdapter.add("${message.arguments[0].asString}: ${message.arguments[1].asString}")
            arrayAdapter.notifyDataSetChanged()
        }
    }

    override fun onConnected() {
        runOnUiThread { Toast.makeText(this@ChatActivity, "Connected", Toast.LENGTH_SHORT).show() }
    }

    override fun onDisconnected() {
        runOnUiThread { Toast.makeText(this@ChatActivity, "Disconnected", Toast.LENGTH_SHORT).show() }
    }

    override fun onMessage(message: HubMessage) {
        runOnUiThread {
            Toast.makeText(this@ChatActivity, "${message.target}\n${Gson().toJson(message.arguments)}", Toast.LENGTH_SHORT).show()
        }
    }

    override fun onError(exception: Exception) {
        runOnUiThread { Toast.makeText(this@ChatActivity, exception.message, Toast.LENGTH_SHORT).show() }
    }
}

