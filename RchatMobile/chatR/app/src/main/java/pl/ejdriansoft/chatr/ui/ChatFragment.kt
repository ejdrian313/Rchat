package pl.ejdriansoft.chatr.ui

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import androidx.fragment.app.Fragment
import kotlinx.android.synthetic.main.fragment_chat.*
import org.jetbrains.anko.doAsync
import org.jetbrains.anko.toast
import org.jetbrains.anko.uiThread
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.services.ChatRAPI

class ChatFragment : Fragment() {

    lateinit var conversationId: String

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        return inflater.inflate(R.layout.fragment_chat, container, false)
    }

    override fun onStart() {
        super.onStart()
        val actChat = (activity as ChatActivity)

        conversationId = arguments?.getString("id")!!

        getMessages(actChat.messageAdapter, conversationId)
        lvMessages.adapter = actChat.messageAdapter

        bSend.setOnClickListener { _ ->
            val message = etMessageText.text.toString()
            etMessageText.setText("")
            try {
                actChat.hubConnection.invoke("Send", message, conversationId)
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    private fun getMessages(adapter: ArrayAdapter<String>, conversationId: String) {
        val call = ChatRAPI(context!!).messages(conversationId)
        doAsync {
            val response = call.execute()
            if (response.isSuccessful) {
                uiThread {
                    adapter.clear()
                    response.body()?.forEach { message ->
                        adapter.add("${message.name}: ${message.body}")

                    }
                    adapter.notifyDataSetChanged()
                    progress?.visibility = View.GONE
                }
            } else {
                uiThread {
                    progress?.visibility = View.GONE
                    (activity as ChatActivity).toast(getString(R.string.error_load_messages)).show()
                }
            }
        }
    }
}
