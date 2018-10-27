package pl.ejdriansoft.chatr.ui


import android.os.Bundle
import android.view.View
import android.widget.ArrayAdapter
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.google.gson.Gson
import com.smartarmenia.dotnetcoresignalrclientjava.*
import kotlinx.android.synthetic.main.activity_chat.*
import org.jetbrains.anko.*
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.services.ChatRAPI
import pl.ejdriansoft.chatr.services.Consts
import pl.ejdriansoft.chatr.services.Prefs
import java.util.ArrayList


class ChatActivity : AppCompatActivity(), HubConnectionListener, HubEventListener, AnkoLogger {

    lateinit var arrayAdapter: ArrayAdapter<String>
    private lateinit var hubConnection: WebSocketHubConnectionP2

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)
        Firebase
        hubConnection = WebSocketHubConnectionP2("https://ejdriansoft.pl/chat", "Bearer ${Consts.token}")

        val messageList = ArrayList<String>()
        arrayAdapter = ArrayAdapter(this@ChatActivity,
                R.layout.item_chat, messageList)
        lvMessages.adapter = arrayAdapter


        getMessages(arrayAdapter)
        setKeyAction()
        startHubConnection()
        subscribeHub()
    }

    private fun subscribeHub() {
        hubConnection.subscribeToEvent("broadcastMessage", this)
        hubConnection.subscribeToEvent("currentConnections") { message ->
            runOnUiThread {
                tvMain.text = "Current connections: ${message.arguments[0].asString}"
            }
        }
    }

    private fun setKeyAction() {
        bSend.setOnClickListener { _ ->
            if (!hubConnection.isConnected) {
                startHubConnection()
                return@setOnClickListener
            }
            val message = etMessageText.text.toString()
            etMessageText.setText("")
            try {
                hubConnection.invoke("Send", message)
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    override fun onResume() {
        super.onResume()
        startHubConnection()
    }

    override fun onStop() {
        super.onStop()
        hubConnection.disconnect()
    }


    private fun getMessages(adapter: ArrayAdapter<String>) {
        val call = ChatRAPI(this).messages()
        doAsync {
            val response = call.execute()
            if (response.isSuccessful) {
                runOnUiThread {
                    response.body()?.forEach {
                        adapter.add("${it.name}: ${it.body}")
                    }
                    adapter.notifyDataSetChanged()
                    progress.visibility = View.GONE
                }
            } else {
                runOnUiThread {
                    progress.visibility = View.GONE
                    tvMain.text = getString(R.string.error_load_messages)
                    toast(getString(R.string.error_load_messages)).show()
                }
            }
        }
    }

    private fun startHubConnection() {
        try {
            hubConnection.connect()
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

