using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV2Service
{
    public enum BrowsingDataKinds
    {
        //
        // Summary:
        //     Specifies file systems data.
        FileSystems,
        //
        // Summary:
        //     Specifies data stored by the IndexedDB DOM feature.
        IndexedDb,
        //
        // Summary:
        //     Specifies data stored by the localStorage DOM API.
        LocalStorage,
        //
        // Summary:
        //     Specifies data stored by the Web SQL database DOM API.
        WebSql,
        //
        // Summary:
        //     Specifies data stored by the CacheStorage DOM API.
        CacheStorage,
        //
        // Summary:
        //     Specifies DOM storage data, now and future. This browsing data kind is inclusive
        //     of COREWEBVIEW2_BROWSING_DATA_KINDS_FILE_SYSTEMS, COREWEBVIEW2_BROWSING_DATA_KINDS_INDEXED_DB,
        //     COREWEBVIEW2_BROWSING_DATA_KINDS_LOCAL_STORAGE, COREWEBVIEW2_BROWSING_DATA_KINDS_WEB_SQL,
        //     COREWEBVIEW2_BROWSING_DATA_KINDS_SERVICE_WORKERS, COREWEBVIEW2_BROWSING_DATA_KINDS_CACHE_STORAGE,
        //     and some other data kinds not listed yet to keep consistent with [DOM-accessible
        //     storage](https://www.w3.org/TR/clear-site-data/#storage).
        AllDomStorage,
        //
        // Summary:
        //     Specifies HTTP cookies data.
        Cookies,
        //
        // Summary:
        //     Specifies all site data, now and future. This browsing data kind is inclusive
        //     of COREWEBVIEW2_BROWSING_DATA_KINDS_ALL_DOM_STORAGE and COREWEBVIEW2_BROWSING_DATA_KINDS_COOKIES.
        //     New site data types may be added to this data kind in the future.
        AllSite,
        //
        // Summary:
        //     Specifies disk cache.
        DiskCache,
        //
        // Summary:
        //     Specifies download history data.
        DownloadHistory,
        //
        // Summary:
        //     Specifies general autofill form data. This excludes password information and
        //     includes information like: names, street and email addresses, phone numbers,
        //     and arbitrary input. This also includes payment data.
        GeneralAutofill,
        //
        // Summary:
        //     Specifies password autosave data.
        PasswordAutosave,
        //
        // Summary:
        //     Specifies browsing history data.
        BrowsingHistory,
        //
        // Summary:
        //     Specifies settings data.
        Settings,
        //
        // Summary:
        //     Specifies profile data that should be wiped to make it look like a new profile.
        //     This does not delete account-scoped data like passwords but will remove access
        //     to account-scoped data by signing the user out. Specifies all profile data, now
        //     and future. New profile data types may be added to this data kind in the future.
        //     This browsing data kind is inclusive of COREWEBVIEW2_BROWSING_DATA_KINDS_ALL_SITE,
        //     COREWEBVIEW2_BROWSING_DATA_KINDS_DISK_CACHE, COREWEBVIEW2_BROWSING_DATA_KINDS_DOWNLOAD_HISTORY,
        //     COREWEBVIEW2_BROWSING_DATA_KINDS_GENERAL_AUTOFILL, COREWEBVIEW2_BROWSING_DATA_KINDS_PASSWORD_AUTOSAVE,
        //     COREWEBVIEW2_BROWSING_DATA_KINDS_BROWSING_HISTORY, and COREWEBVIEW2_BROWSING_DATA_KINDS_SETTINGS.
        AllProfile,
        //
        // Summary:
        //     Specifies service workers registered for an origin, and clear will result in
        //     termination and deregistration of them.
        ServiceWorkers
    }
    public enum ColorScheme
    {
        Auto,
        Light,
        Dark
    }
}
