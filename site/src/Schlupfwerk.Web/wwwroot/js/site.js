// Small helpers for the site. Loaded before Blazor so "/" and "/cafe" redirect to the right
// language without waiting for WebAssembly to download. No tracking, no third parties.
(function () {
  "use strict";

  var KEY = "lang";

  function stored() {
    try {
      var value = window.localStorage.getItem(KEY);
      return value === "pt" || value === "en" ? value : null;
    } catch (e) {
      return null; // Storage blocked (private mode, strict settings): fall back to the browser language.
    }
  }

  function preferredLang() {
    var saved = stored();
    if (saved) {
      return saved;
    }
    var first = (navigator.languages && navigator.languages[0]) || navigator.language || "";
    return first.toLowerCase().indexOf("pt") === 0 ? "pt" : "en";
  }

  window.schlupfwerk = {
    preferredLang: preferredLang,
    saveLang: function (lang) {
      try {
        window.localStorage.setItem(KEY, lang);
      } catch (e) {
        // Not available: the choice just isn't remembered.
      }
    },
    setDocumentLang: function (lang) {
      document.documentElement.lang = lang;
    }
  };

  var path = window.location.pathname.replace(/\/+$/, "");
  if (path === "") {
    window.location.replace("/" + preferredLang());
  } else if (path === "/cafe") {
    window.location.replace("/" + preferredLang() + "/cafe");
  }
})();
