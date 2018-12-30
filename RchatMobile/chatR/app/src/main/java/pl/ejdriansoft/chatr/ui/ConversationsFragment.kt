package pl.ejdriansoft.chatr.ui

import android.app.AlertDialog
import android.content.DialogInterface
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.EditText
import android.widget.LinearLayout
import android.widget.Toast
import androidx.fragment.app.Fragment
import kotlinx.android.synthetic.main.fragment_conversation.*
import org.jetbrains.anko.*
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.data.Conversations
import pl.ejdriansoft.chatr.services.ChatRAPI
import pl.ejdriansoft.chatr.ui.adapters.ConversationAdapter
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import pl.ejdriansoft.chatr.data.CreateConversation


class ConversationsFragment : Fragment(), AnkoLogger {


    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        return inflater.inflate(R.layout.fragment_conversation, container, false)
    }

    override fun onStart() {
        super.onStart()
        val llm = LinearLayoutManager(context!!)
        val api = ChatRAPI(context!!)

        llm.orientation = RecyclerView.VERTICAL
        lvConversations.layoutManager = llm
        lvConversations.adapter = ConversationAdapter((activity as ChatActivity).conversations, context!!, fragmentManager!!)
        getConversations((activity as ChatActivity).conversations, api)
        addConv.setOnClickListener {

            val alertDialog = AlertDialog.Builder(context!!)
            alertDialog.setTitle("Add conversation");
            alertDialog.setMessage("Enter email of your friend:");

            val input =  EditText(context!!)
            val lp = LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.MATCH_PARENT,
                    LinearLayout.LayoutParams.MATCH_PARENT)
            input.layoutParams = lp
            alertDialog.setView(input)
            alertDialog.setPositiveButton("Send") { dialog, which ->
                val call = api.createConversation(CreateConversation(input.text.toString()))
                doAsync {
                    val response = call.execute()
                    if (response.isSuccessful) {
                        getConversations((activity as ChatActivity).conversations, api)
                    } else {
                        uiThread {
                            Toast.makeText(context!!, "Error", Toast.LENGTH_LONG)
                        }
                    }
                }


                dialog.dismiss()
            }
            alertDialog.setNegativeButton("Close") { dialog, which ->
                dialog.dismiss()
            }

            alertDialog.show()
        }
    }

    private fun getConversations(list: ArrayList<Conversations>, api: ChatRAPI) {
        val call = api.conversations()
        doAsync {
            val response = call.execute()
            if (response.isSuccessful) {
                uiThread {
                    list.clear()
                    progress?.visibility = View.GONE
                    response.body()?.forEach {conversation ->
                        list.add(conversation)
                        (activity as ChatActivity).conversationsHashMap.put(conversation, null)
                    }
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
