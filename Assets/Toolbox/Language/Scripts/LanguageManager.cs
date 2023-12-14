using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Runtime.InteropServices;


public class LanguageManager : MonoBehaviour
{
    public enum Language
    {
        // ab, // Abkhazian	
        // aa, // Afar	
        // af, // Afrikaans	
        // ak, // Akan	
        // sq, // Albanian	
        // am, // Amharic	
        // ar, // Arabic	
        // an, // Aragonese	
        // hy, // Armenian	
        //     // as, // Assamese	
        // av, // Avaric	
        // ae, // Avestan	
        // ay, // Aymara	
        // az, // Azerbaijani	
        // bm, // Bambara	
        // ba, // Bashkir	
        // eu, // Basque	
        // be, // Belarusian	
        // bn, // Bengali (Bangla)	
        // bh, // Bihari	
        // bi, // Bislama	
        // bs, // Bosnian	
        // br, // Breton	
        // bg, // Bulgarian	
        // my, // Burmese	
        // ca, // Catalan	
        // ch, // Chamorro	
        // ce, // Chechen	
        // ny, // Chichewa, Chewa, Nyanja	
        // zh, // Chinese	
        // cv, // Chuvash	
        // kw, // Cornish	
        // co, // Corsican	
        // cr, // Cree	
        // hr, // Croatian	
        // cs, // Czech	
        // da, // Danish	
        // dv, // Divehi, Dhivehi, Maldivian	
        // nl, // Dutch	
        // dz, // Dzongkha	
        en, // English	
        // eo, // Esperanto	
        // et, // Estonian	
        // ee, // Ewe	
        // fo, // Faroese	
        // fj, // Fijian	
        // fi, // Finnish	
        fr, // French	
        // ff, // Fula, Fulah, Pulaar, Pular	
        // gl, // Galician	
        // gd, // Gaelic (Scottish)	
        // gv, // Gaelic (Manx)	
        // ka, // Georgian	
        // de, // German	
        // el, // Greek	
        // kl, // Greenlandic	
        // gn, // Guarani	
        // gu, // Gujarati	
        // ht, // Haitian Creole	
        // ha, // Hausa	
        // he, // Hebrew	
        // hz, // Herero	
        // hi, // Hindi	
        // ho, // Hiri Motu	
        // hu, // Hungarian	
        //     // is, // Icelandic	
        // io, // Ido	
        // ig, // Igbo	
        // id, // Indonesian
        //     // in, // Indonesian
        // ia, // Interlingua	
        // ie, // Interlingue	
        // iu, // Inuktitut	
        // ik, // Inupiak	
        // ga, // Irish	
        // it, // Italian	
        // ja, // Japanese	
        // jv, // Javanese	
        //     // kl, // Kalaallisut, Greenlandic	
        // kn, // Kannada	
        // kr, // Kanuri	
        // ks, // Kashmiri	
        // kk, // Kazakh	
        // km, // Khmer	
        // ki, // Kikuyu	
        // rw, // Kinyarwanda (Rwanda)	
        // rn, // Kirundi	
        // ky, // Kyrgyz	
        // kv, // Komi	
        // kg, // Kongo	
        // ko, // Korean	
        // ku, // Kurdish	
        // kj, // Kwanyama	
        // lo, // Lao	
        // la, // Latin	
        // lv, // Latvian (Lettish)	
        // li, // Limburgish ( Limburger)	
        // ln, // Lingala	
        // lt, // Lithuanian	
        // lu, // Luga-Katanga	
        // lg, // Luganda, Ganda	
        // lb, // Luxembourgish	
        //     // gv, // Manx	
        // mk, // Macedonian	
        // mg, // Malagasy	
        // ms, // Malay	
        // ml, // Malayalam	
        // mt, // Maltese	
        // mi, // Maori	
        // mr, // Marathi	
        // mh, // Marshallese	
        // mo, // Moldavian	
        // mn, // Mongolian	
        // na, // Nauru	
        // nv, // Navajo	
        // ng, // Ndonga	
        // nd, // Northern Ndebele	
        // ne, // Nepali	
        // no, // Norwegian	
        // nb, // Norwegian bokmål	
        // nn, // Norwegian nynorsk	
        // ii, // Nuosu	
        // oc, // Occitan	
        // oj, // Ojibwe	
        // cu, // Old Church Slavonic, Old Bulgarian	
        // or, // Oriya	
        // om, // Oromo (Afaan Oromo)	
        // os, // Ossetian	
        // pi, // Pāli	
        // ps, // Pashto, Pushto	
        // fa, // Persian (Farsi)	
        // pl, // Polish	
        pt, // Portuguese	
        // pa, // Punjabi (Eastern)	
        // qu, // Quechua	
        // rm, // Romansh	
        // ro, // Romanian	
        // ru, // Russian	
        // se, // Sami	
        // sm, // Samoan	
        // sg, // Sango	
        // sa, // Sanskrit	
        // sr, // Serbian	
        // sh, // Serbo-Croatian	
        // st, // Sesotho	
        // tn, // Setswana	
        // sn, // Shona	
        //     // ii, // Sichuan Yi	
        // sd, // Sindhi	
        // si, // Sinhalese	
        // ss, // Siswati	
        // sk, // Slovak	
        // sl, // Slovenian	
        // so, // Somali	
        // nr, // Southern Ndebele	
        es, // Spanish	
        // su, // Sundanese	
        // sw, // Swahili (Kiswahili)	
        //     // ss, // Swati	
        // sv, // Swedish	
        // tl, // Tagalog	
        // ty, // Tahitian	
        // tg, // Tajik	
        // ta, // Tamil	
        // tt, // Tatar	
        // te, // Telugu	
        // th, // Thai	
        // bo, // Tibetan	
        // ti, // Tigrinya	
        // to, // Tonga	
        // ts, // Tsonga	
        // tr, // Turkish	
        // tk, // Turkmen	
        // tw, // Twi	
        // ug, // Uyghur	
        // uk, // Ukrainian	
        // ur, // Urdu	
        // uz, // Uzbek	
        // ve, // Venda	
        // vi, // Vietnamese	
        // vo, // Volapük	
        // wa, // Wallon	
        // cy, // Welsh	
        // wo, // Wolof	
        // fy, // Western Frisian	
        // xh, // Xhosa	
        // yi, // Yiddish
        // ji, // Yiddish
        // yo, // Yoruba	
        // za, // Zhuang, Chuang	
        // zu, // Zulu	
    }
    public static LanguageManager Instance;
    [SerializeField]
    private Language lang = Language.en;
    public static Language language { get { return Instance.lang; } }
    public UnityEvent<Language> onLanguageChanged = new UnityEvent<Language>();

    [SerializeField]
    private List<MultiLanguageElement> elements = new List<MultiLanguageElement>();


    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string getSpraoiLoadingData();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string subscribeToLanguageAndRegionEvents();

    // [DllImport("__Internal")]
    // private static extern string GetLocale();

    private string reg = "us";
    public static string region { get { return Instance.reg; } }
    public UnityEvent<string> onRegionChanged = new UnityEvent<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
#if UNITY_WEBGL && !UNITY_EDITOR
            subscribeToLanguageAndRegionEvents();
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // try {
        //     string windowData = getSpraoiLoadingData();
        //     WindowSpraoi loadingPageData = JsonUtility.FromJson<WindowSpraoi>(windowData);
        //     System.Enum.TryParse<Language>(loadingPageData.language, out this.lang);
        //     this.reg = loadingPageData.region;
        // } catch (System.Exception e) {
        //     Debug.Log($"[LanguageManager] Failed to get region and language from loading page. {e.Message}");
            this.lang = Language.en;
            this.reg = "us";
        // }
#else
        this.lang = Language.en;
        this.reg = "us";
#endif
    }

    private Language _debug_last;
    private void Update()
    {
        if (_debug_last != lang)
        {
            _debug_last = lang;
            onLanguageChanged.Invoke(this.lang);
        }
    }

    public bool SetLanguage(string language)
    {
        try
        {
            this.lang = (Language)System.Enum.Parse(typeof(Language), language);
            onLanguageChanged.Invoke(this.lang);
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public bool SetRegion(string region)
    {
        try
        {
            this.reg = region;
            onRegionChanged.Invoke(this.reg);
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public static void OnLanguageChanged(System.Action<Language> action)
    {
        Instance.onLanguageChanged.AddListener((lang) => action(lang));
    }

    public static void OnRegionChanged(System.Action<string> action)
    {
        Instance.onRegionChanged.AddListener((reg) => action(reg));
    }


    public void AddElement(MultiLanguageElement element)
    {
        if (!elements.Contains(element))
            elements.Add(element);
    }

    public void RemoveElement(MultiLanguageElement element)
    {
        if (elements.Contains(element))
            elements.Remove(element);
    }




    [System.Serializable]
    public class LanguageResource
    {
        public Language language;
        public TextAsset textAsset;
    }

    [System.Serializable]
    public class LanguageString : LanguageElement
    {
        public string text;

        public LanguageString(string language, string text)
        {
            this.language = (Language)System.Enum.Parse(typeof(Language), language);
            this.text = text;
        }

        public LanguageString(Language language, string text)
        {
            this.language = language;
            this.text = text;
        }
    }

    [System.Serializable]
    public class LanguageStringArray : LanguageElement
    {
        public string[] texts;

        public LanguageStringArray(string language, string text)
        {
            this.language = (Language)System.Enum.Parse(typeof(Language), language);
            this.texts = new string[] { text };
        }

        public LanguageStringArray(Language language, string text)
        {
            this.language = language;
            this.texts = new string[] { text };
        }

        public LanguageStringArray(Language language, string[] texts)
        {
            this.language = language;
            this.texts = texts;
        }
    }

    [System.Serializable]
    public abstract class LanguageElement
    {
        public Language language;
    }

    public static Dictionary<string, string> LanguageCodes = new Dictionary<string, string>() {
        {"ab", "Abkhazian"},
        {"aa", "Afar"},
        {"af", "Afrikaans"},
        {"ak", "Akan"},
        {"sq", "Albanian"},
        {"am", "Amharic"},
        {"ar", "Arabic"},
        {"an", "Aragonese"},
        {"hy", "Armenian"},
        {"as", "Assamese"},
        {"av", "Avaric"},
        {"ae", "Avestan"},
        {"ay", "Aymara"},
        {"az", "Azerbaijani"},
        {"bm", "Bambara"},
        {"ba", "Bashkir"},
        {"eu", "Basque"},
        {"be", "Belarusian"},
        {"bn", "Bengali (Bangla)"},
        {"bh", "Bihari"},
        {"bi", "Bislama"},
        {"bs", "Bosnian"},
        {"br", "Breton"},
        {"bg", "Bulgarian"},
        {"my", "Burmese"},
        {"ca", "Catalan"},
        {"ch", "Chamorro"},
        {"ce", "Chechen"},
        {"ny", "Chichewa, Chewa, Nyanja"},
        {"zh", "Chinese"},
        {"cv", "Chuvash"},
        {"kw", "Cornish"},
        {"co", "Corsican"},
        {"cr", "Cree"},
        {"hr", "Croatian"},
        {"cs", "Czech"},
        {"da", "Danish"},
        {"dv", "Divehi, Dhivehi, Maldivian"},
        {"nl", "Dutch"},
        {"dz", "Dzongkha"},
        {"en", "English"},
        {"eo", "Esperanto"},
        {"et", "Estonian"},
        {"ee", "Ewe"},
        {"fo", "Faroese"},
        {"fj", "Fijian"},
        {"fi", "Finnish"},
        {"fr", "French"},
        {"ff", "Fula, Fulah, Pulaar, Pular"},
        {"gl", "Galician"},
        {"gd", "Gaelic (Scottish)"},
        {"gv", "Gaelic (Manx)"},
        // {"gv", "Manx"},
        {"ka", "Georgian"},
        {"de", "German"},
        {"el", "Greek"},
        // {"kl", "Greenlandic"},
        {"kl", "Kalaallisut, Greenlandic"},
        {"gn", "Guarani"},
        {"gu", "Gujarati"},
        {"ht", "Haitian Creole"},
        {"ha", "Hausa"},
        {"he", "Hebrew"},
        {"hz", "Herero"},
        {"hi", "Hindi"},
        {"ho", "Hiri Motu"},
        {"hu", "Hungarian"},
        {"is", "Icelandic"},
        {"io", "Ido"},
        {"ig", "Igbo"},
        {"id", "Indonesian"},
        {"in", "Indonesian"},
        {"ia", "Interlingua"},
        {"ie", "Interlingue"},
        {"iu", "Inuktitut"},
        {"ik", "Inupiak"},
        {"ga", "Irish"},
        {"it", "Italian"},
        {"ja", "Japanese"},
        {"jv", "Javanese"},
        {"kn", "Kannada"},
        {"kr", "Kanuri"},
        {"ks", "Kashmiri"},
        {"kk", "Kazakh"},
        {"km", "Khmer"},
        {"ki", "Kikuyu"},
        {"rw", "Kinyarwanda (Rwanda)"},
        {"rn", "Kirundi"},
        {"ky", "Kyrgyz"},
        {"kv", "Komi"},
        {"kg", "Kongo"},
        {"ko", "Korean"},
        {"ku", "Kurdish"},
        {"kj", "Kwanyama"},
        {"lo", "Lao"},
        {"la", "Latin"},
        {"lv", "Latvian (Lettish)"},
        {"li", "Limburgish ( Limburger)"},
        {"ln", "Lingala"},
        {"lt", "Lithuanian"},
        {"lu", "Luga-Katanga"},
        {"lg", "Luganda, Ganda"},
        {"lb", "Luxembourgish"},
        {"mk", "Macedonian"},
        {"mg", "Malagasy"},
        {"ms", "Malay"},
        {"ml", "Malayalam"},
        {"mt", "Maltese"},
        {"mi", "Maori"},
        {"mr", "Marathi"},
        {"mh", "Marshallese"},
        {"mo", "Moldavian"},
        {"mn", "Mongolian"},
        {"na", "Nauru"},
        {"nv", "Navajo"},
        {"ng", "Ndonga"},
        {"nd", "Northern Ndebele"},
        {"ne", "Nepali"},
        {"no", "Norwegian"},
        {"nb", "Norwegian bokmål"},
        {"nn", "Norwegian nynorsk"},
        // {"ii", "Nuosu"},
        {"ii", "Sichuan Yi"},
        {"oc", "Occitan"},
        {"oj", "Ojibwe"},
        {"cu", "Old Church Slavonic, Old Bulgarian"},
        {"or", "Oriya"},
        {"om", "Oromo (Afaan Oromo)"},
        {"os", "Ossetian"},
        {"pi", "Pāli"},
        {"ps", "Pashto, Pushto"},
        {"fa", "Persian (Farsi)"},
        {"pl", "Polish"},
        {"pt", "Portuguese"},
        {"pa", "Punjabi (Eastern)"},
        {"qu", "Quechua"},
        {"rm", "Romansh"},
        {"ro", "Romanian"},
        {"ru", "Russian"},
        {"se", "Sami"},
        {"sm", "Samoan"},
        {"sg", "Sango"},
        {"sa", "Sanskrit"},
        {"sr", "Serbian"},
        {"sh", "Serbo-Croatian"},
        {"st", "Sesotho"},
        {"tn", "Setswana"},
        {"sn", "Shona"},
        {"sd", "Sindhi"},
        {"si", "Sinhalese"},
        {"ss", "Siswati"},
        // {"ss", "Swati"},
        {"sk", "Slovak"},
        {"sl", "Slovenian"},
        {"so", "Somali"},
        {"nr", "Southern Ndebele"},
        {"es", "Spanish"},
        {"su", "Sundanese"},
        {"sw", "Swahili (Kiswahili)"},
        {"sv", "Swedish"},
        {"tl", "Tagalog"},
        {"ty", "Tahitian"},
        {"tg", "Tajik"},
        {"ta", "Tamil"},
        {"tt", "Tatar"},
        {"te", "Telugu"},
        {"th", "Thai"},
        {"bo", "Tibetan"},
        {"ti", "Tigrinya"},
        {"to", "Tonga"},
        {"ts", "Tsonga"},
        {"tr", "Turkish"},
        {"tk", "Turkmen"},
        {"tw", "Twi"},
        {"ug", "Uyghur"},
        {"uk", "Ukrainian"},
        {"ur", "Urdu"},
        {"uz", "Uzbek"},
        {"ve", "Venda"},
        {"vi", "Vietnamese"},
        {"vo", "Volapük"},
        {"wa", "Wallon"},
        {"cy", "Welsh"},
        {"wo", "Wolof"},
        {"fy", "Western Frisian"},
        {"xh", "Xhosa"},
        {"yi", "Yiddish"},
        {"ji", "Yiddish"},
        {"yo", "Yoruba"},
        {"za", "Zhuang, Chuang"},
        {"zu", "Zulu"},
    };
    public static Dictionary<string, string> CountryCodes = new Dictionary<string, string>() {
        {"US", "United States of America"},
        {"CA", "Canada"},
        {"MX", "Mexico"}
        // TODO: complete this list
    };

    public enum CountryCode
    {
        US,
        CA,
        MX
    }

    public struct WindowSpraoi
    {
        public string language;
        public string region;
    }
}
