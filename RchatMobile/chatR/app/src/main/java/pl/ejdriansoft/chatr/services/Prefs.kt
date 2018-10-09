package pl.ejdriansoft.chatr.services.api

import android.content.Context
import pl.ejdriansoft.chatr.R

class Prefs(private val context: Context) {

    private val prefs = context.getSharedPreferences(context.getString(R.string.preference_file_key), 0)

    var nickname: String?
        get() = prefs.getString(context.getString(R.string.nickname), "")
        set(value) = prefs.edit().putString(context.getString(R.string.nickname), value).apply()
}