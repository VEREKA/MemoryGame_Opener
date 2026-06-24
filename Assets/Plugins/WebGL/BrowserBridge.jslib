mergeInto(LibraryManager.library, {
    SendPlayerResult: function(nickPtr, timePtr) {
        console.log("BrowserBridge.SendPlayerResult called");

        var nick = UTF8ToString(nickPtr);
        var time = UTF8ToString(timePtr);

        console.log("BrowserBridge result:", nick, time);

        try {
            if (typeof window.SetUnityPlayerResult === 'function') {
                console.log('BrowserBridge: calling window.SetUnityPlayerResult');
                window.SetUnityPlayerResult(nick, time);
                return;
            }
        } catch (e) {
            console.error('BrowserBridge: error calling window.SetUnityPlayerResult', e);
        }

        try {
            if (window.parent && typeof window.parent.SetUnityPlayerResult === 'function') {
                console.log('BrowserBridge: calling window.parent.SetUnityPlayerResult');
                window.parent.SetUnityPlayerResult(nick, time);
                return;
            }
        } catch (e) {
            console.error('BrowserBridge: error calling window.parent.SetUnityPlayerResult', e);
        }

        try {
            if (typeof window.updateScoreFromUnity === 'function') {
                console.log('BrowserBridge: calling window.updateScoreFromUnity');
                window.updateScoreFromUnity(time);
                return;
            }
        } catch (e) {
            console.error('BrowserBridge: error calling window.updateScoreFromUnity', e);
        }

        try {
            if (window.parent && typeof window.parent.updateScoreFromUnity === 'function') {
                console.log('BrowserBridge: calling window.parent.updateScoreFromUnity');
                window.parent.updateScoreFromUnity(time);
                return;
            }
        } catch (e) {
            console.error('BrowserBridge: error calling window.parent.updateScoreFromUnity', e);
        }

        try {
            if (window.parent) {
                console.log('BrowserBridge: posting message to parent');
                window.parent.postMessage({ unityGameResult: true, nick: nick, time: time }, '*');
                return;
            }
        } catch (e) {
            console.error('BrowserBridge: error posting message to parent', e);
        }

        console.warn('BrowserBridge: no supported callback defined on window or parent window.');
    }
});
