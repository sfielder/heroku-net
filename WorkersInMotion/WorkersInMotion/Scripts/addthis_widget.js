if (!((window._atc || {}).ver))
{
    var _atd = "www.addthis.com/",
    _atr = "//s7.addthis.com/",
    _euc = encodeURIComponent,
    _duc = decodeURIComponent,
    _atc =
    {
        rrev: 114377,
        dr: 0,
        ver: 250,
        loc: 0,
        enote: "",
        cwait: 500,
        bamp: 0.25,
        camp: 1,
        csmp: 0.0001,
        damp: 0,
        famp: 0.02,
        pamp: 0.2,
        tamp: 1,
        lamp: 1,
        vamp: 1,
        vrmp: 0.0001,
        ohmp: 0,
        ltj: 1,
        xamp: 1,
        abf: !!window.addthis_do_ab,
        qs: 0,
        cdn: 0,
        rsrcs: {
            bookmark: _atr + "static/r07/bookmark013.html",
            atimg: _atr + "static/r07/atimg013.html",
            countercss: _atr + "static/r07/counter002.css",
            counterIE67css: _atr + "static/r07/counterIE67002.css",
            counter: _atr + "static/r07/counter002.js",
            wombat: _atr + "static/r07/bar006.js",
            qbarcss: _atr + "bannerQuirks.css",
            fltcss: _atr + "static/r07/floating001.css",
            barcss: _atr + "static/r07/banner004.css",
            barjs: _atr + "static/r07/banner001.js",
            contentcss: _atr + "static/r07/content005.css",
            contentjs: _atr + "static/r07/content005.js",
            ssojs: _atr + "static/r07/sso000.js",
            ssocss: _atr + "static/r07/sso000.css",
            peekaboocss: _atr + "static/r07/peekaboo001.css",
            overlayjs: _atr + "static/r07/overlay004.js",
            widget32css: _atr + "static/r07/widgetbig045.css",
            widgetcss: _atr + "static/r07/widget084.css",
            widgetIE67css: _atr + "static/r07/widgetIE67002.css",
            widgetpng: "//s7.addthis.com/static/r07/widget045.png",
            link: _atr + "static/r07/link.html",
            pinit: _atr + "static/r07/pinit005.html",
            linkedin: _atr + "static/r07/linkedin018.html",
            tweet: _atr + "static/r07/tweet018.html",
            menujs: "static/r07/menu113.js",
            sh: _atr + "static/r07/sh089.html"
        }
    };
}
(function () 
{ 
    var e; 
    var h = (window.location.protocol == "https:"), 
    o, j, 
    l = 
        { 
            0: _atr, 1: "//fastly.addthis.com", 
            2: "//level3.addthis.com" }, 
            q = Math.random(); 
            _atc.cdn = 0; 
            if (!window.addthis || window.addthis.nodeType !== e) 
            { 
                function g(u, s, r, a) 
                { 
                    return function () 
                    { 
                        if (!this.qs) 
                        { 
                            this.qs = 0; 
                        } _atc.qs++; 
                        if (!((this.qs++ > 0 && a) || _atc.qs > 1000) && window.addthis) 
                        { 
                            window.addthis.plo.push({ call: u, args: arguments, ns: s, ctx: r }); 
                        } 
                    }; 
                } 
                function n(s) 
                { 
                    var r = this, 
                    a = this.queue = []; 
                    this.name = s; 
                    this.call = function () { a.push(arguments); }; 
                    this.call.queuer = this; 
                    this.flush = function (w, v) 
                                    { 
                                        this.flushed = 1; 
                                        for (var u = 0; u < a.length; u++) 
                                        { 
                                            w.apply(v || r, a[u]); 
                                        } 
                                        return w; 
                                    }; 
                } 
                window.addthis = { 
                                    ost: 0, 
                                    cache: {}, 
                                    plo: [], 
                                    links: [], 
                                    ems: [], 
                                    timer: { 
                                            load: ((new Date()).getTime()) }, 
                                            _Queuer: n, 
                                            _queueFor: g, 
                                            data: { 
                                                    getShareCount: g("getShareCount", "data") }, 
                                                    bar: { show: g("show", "bar"), 
                                                    initialize: g("initialize", "bar") }, 
                                                    login: { initialize: g("initialize", "login"), 
                                                    connect: g("connect", "login") }, 
                                                    box: g("box"), 
                                                    button: g("button"), 
                                                    counter: g("counter"), 
                                                    count: g("count"), 
                                                    toolbox: g("toolbox"), 
                                                    update: g("update"), 
                                                    init: g("init"), 
                                                    ad: { 
                                                            event: g("event", "ad"), 
                                                            getPixels: g("getPixels", "ad") }, 
                                                            util: { 
                                                                    getServiceName: g("getServiceName") }, 
                                                                    ready: g("ready"), 
                                                                    addEventListener: g("addEventListener", "ed", "ed"), 
                                                                    removeEventListener: g("removeEventListener", "ed", "ed"), 
                                                                    user: { 
                                                                                getID: g("getID", "user"), 
                                                                                getGeolocation: g("getGeolocation", "user", null, true), 
                                                                                getPreferredServices: g("getPreferredServices", "user", null, true), 
                                                                                getServiceShareHistory: g("getServiceShareHistory", "user", null, true), 
                                                                                ready: g("ready", "user"), isReturning: g("isReturning", "user"), 
                                                                                isOptedOut: g("isOptedOut", "user"), 
                                                                                isUserOf: g("isUserOf", "user"), 
                                                                                hasInterest: g("hasInterest", "user"), 
                                                                                isLocatedIn: g("isLocatedIn", "user"), 
                                                                                interests: g("getInterests", "user"), 
                                                                                services: g("getServices", "user"), 
                                                                                location: g("getLocation", "user") }, 
                                                                                session: { source: g("getSource", "session"), 
                                                                                isSocial: g("isSocial", "session"), 
                                                                                isSearch: g("isSearch", "session") }, 
                                                                                _pmh: new n("pmh") 
                                                                            }; 
                                                                        var t = document.getElementsByTagName("script")[0]; 
                                                                        function f(a) 
                                                                        { 
                                                                            a.style.width = a.style.height = "1px"; 
                                                                            a.style.position = "absolute"; 
                                                                            a.style.zIndex = 100000; 
                                                                        } 
                                                                        if (document.location.href.indexOf(_atr) == -1) 
                                                                        { 
                                                                            var d = document.getElementById("_atssh"); 
                                                                            if (!d) 
                                                                            { 
                                                                                d = document.createElement("div"); 
                                                                                d.style.visibility = "hidden"; 
                                                                                d.id = "_atssh"; 
                                                                                f(d); t.parentNode.appendChild(d); 
                                                                            } 
                                                                            function i(a) 
                                                                            { 
                                                                                if (addthis._pmh.flushed) 
                                                                                { 
                                                                                    _ate.pmh(a); 
                                                                                } 
                                                                                else 
                                                                                { 
                                                                                    addthis._pmh.call(a); 
                                                                                } 
                                                                            } 
                                                                            if (window.postMessage) 
                                                                            { 
                                                                                if (window.attachEvent) 
                                                                                { 
                                                                                    window.attachEvent("onmessage", i); 
                                                                                } 
                                                                                else 
                                                                                { 
                                                                                    if (window.addEventListener) 
                                                                                    { 
                                                                                        window.addEventListener("message", i, false); 
                                                                                    } 
                                                                                } 
                                                                            } 
                                                                            if (!d.firstChild) 
                                                                            { 
                                                                                var k, c = navigator.userAgent.toLowerCase(), 
                                                                                    b = Math.floor(Math.random() * 1000); 
                                                                                if (/msie/.test(c) && !(/opera/.test(c))) 
                                                                                { 
                                                                                    d.innerHTML = "<iframe id=\"_atssh" + b + "\" width=\"1\" height=\"1\" title=\"AddThis utility frame\" name=\"_atssh" + b + "\" src=\"\">"; 
                                                                                    k = document.getElementById("_atssh" + b); 
                                                                                } 
                                                                                else 
                                                                                { 
                                                                                    k = document.createElement("iframe"); 
                                                                                    k.id = "_atssh" + b; 
                                                                                    k.title = "AddThis utility frame"; 
                                                                                    d.appendChild(k); } f(k); 
                                                                                    k.frameborder = k.style.border = 0; 
                                                                                    k.style.top = k.style.left = 0; 
                                                                                    _atc._atf = k; 
                                                                                } 
                                                                            } 
                                                                            var p = document.createElement("script"); 
                                                                                p.type = "text/javascript"; p.src = (h ? "https:" : "http:") + "//s7.addthis.com/static/r07/core016.js"; 
                                                                                t.parentNode.appendChild(p); 
                                                                                var m = 10000; 
                                                                            if (Math.random() < _atc.ohmp) 
                                                                            { 
                                                                                setTimeout(function () 
                                                                                            { 
                                                                                                if (!window.addthis.timer.core) 
                                                                                                { 
                                                                                                    (new Image()).src = "//m.addthisedge.com/live/t00/oh.gif?" + Math.floor(Math.random() * 4294967295).toString(36) + "&cdn=" + _atc.cdn + "&sr=" + _atc.ohmp + "&rev=" + _atc.rrev + "&to=" + m; 
                                                                                                } 
                                                                                            }
                                                                                            , m); 
                                                                            } 
                                                                        } 
                                                                    })();