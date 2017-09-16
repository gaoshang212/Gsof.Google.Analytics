using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chsword;
using Gsof.Extensions;

namespace Gsof.Google.Analytics
{
    public class Analytics
    {
        // Debug
        private readonly bool _debug;
        private readonly string _userAgent;
        private const string BaseUrl = "https://www.google-analytics.com";

        private const string DebugPath = "/debug";
        private const string CollectPath = "/collect";
        private const string BatchPath = "/batch";

        // Google generated ID
        private readonly string _tid;
        // Google API version
        private readonly int _version;

        // Clinet Id
        private readonly string _cid;

        private readonly Queue<ITracker> _queue = new Queue<ITracker>();

        private readonly string _proxy;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="tid">Tracking ID</param>
        /// <param name="cid">Anonymous Client ID</param>
        /// <param name="userAgent">User Agent</param>
        /// <param name="debug">debug</param>
        /// <param name="proxy">proxy</param>
        /// <param name="version">Version</param>
        public Analytics(string tid, string cid, string userAgent = "", bool debug = false, string proxy = "", int version = 1)
        {
            this._debug = debug;
            // User-agent
            this._userAgent = userAgent;
            // Google generated ID
            this._tid = tid;
            // Google API version
            this._version = version;
            this._cid = cid;
            this._proxy = proxy;

            if (string.IsNullOrEmpty(this._cid))
            {
                this._cid = Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// pageview
        /// </summary>
        /// <param name="hostname">Document hostname</param>
        /// <param name="url">Page</param>
        /// <param name="title">Title</param>
        /// <returns></returns>
        public Analytics Pageview(string hostname, string url, string title)
        {
            var parameters = this.CreateTracker("pageview", new { dh = hostname, dp = url, dt = title });
            this.Push(parameters);
            //return this.Send(params);
            return this;
        }

        /// <summary>
        /// Event
        /// </summary>
        /// <param name="category">Event Category.</param>
        /// <param name="action">Event Action.</param>
        /// <param name="label">Event label.</param>
        /// <param name="value">Event value.</param>
        /// <returns></returns>
        public Analytics Event(string category, string action, string label = null, string value = null)
        {
            var parameters = new
            {
                ec = category,
                ea = action,
                el = label,
                ev = value,
            };

            this.Push(this.CreateTracker("event", parameters));
            return this;
        }

        /**
         * screenview
         * @param name App name
         * @param ver App version
         * @param id App Id
         * @param aiid App Installer Id
         * @param path Screen name
         * @param opts Custom param
         */
        public Analytics Screenview(string name, string ver, string id, string aiid, string path)
        {
            var parameters = this.CreateTracker("screenview", new
            {
                an = name,
                av = ver,
                aid = id,
                aiid,
                cd = path
            });

            this.Push(parameters);
            return this;
        }

        /**
         *  a "transaction" request
         * @param trnID Transaction ID.
         * @param trnAffil Transaction affiliation.
         * @param trnRev Transaction revenue.
         * @param trnShip Transaction shipping.
         * @param trnTax Transaction tax.
         * @param currCode Currency code.
         */
        public Analytics Transaction(string trnID, string trnAffil, string trnRev, string trnShip, string trnTax, string currCode)
        {
            var parameters = new
            {
                ti = trnID,
                ta = trnAffil,
                tr = trnRev,
                ts = trnShip,
                tt = trnTax,
                cu = currCode,
            };

            this.Push(this.CreateTracker("transaction", parameters));
            return this;
        }

        /**
         * a "social" request
         * @param socialAction Social Action. Required.
         * @param socialNetwork Social Network. Required.
         * @param socialTarget Social Target. Required.
         */
        public Analytics Social(string socialAction, string socialNetwork, string socialTarget)
        {
            var parameters = this.CreateTracker("social", new { sa = socialAction, sn = socialNetwork, st = socialTarget });
            this.Push(parameters);
            //return this.Send(params);
            return this;
        }

        // /**
        //  * Send a "exception" request
        //  *
        //  * @param  {string} exDesc   Exception description
        //  * @param  {Number} exFatal  Exception is fatal?
        //  * @param  {string} clientID uuidV4
        //  *
        //  * @return {Promise}
        //  */

        /**
         * a "exception" request
         * @param exDesc Exception description.
         * @param exFatal Exception is fatal?
         */
        public Analytics Exception(string exDesc, bool exFatal)
        {
            var parameters = this.CreateTracker("exception", new { exd = exDesc, exf = exFatal });

            //return this.Send(params);
            this.Push(parameters);
            return this;
        }

        /**
         * 
         * @param trnID Transaction ID. Required.
         * @param evCategory Event Category. Required.
         * @param evAction Event Action. Required.
         * @param nonInteraction Non-interaction parameter.
         */
        public Analytics Refund(string trnID, string evCategory = "Ecommerce", string evAction = "Refund", int nonInteraction = 1)
        {
            var parameters = this.CreateTracker("event", new
            {
                ec = evCategory,
                ea = evAction,
                ni = nonInteraction,
                ti = trnID,
                pa = "refund"
            });

            //return this.Send(params);
            this.Push(parameters);
            return this;
        }

        // /**
        //  * Send a "item" request
        //  * @param  {string} trnID         Transaction ID
        //  * @param  {string} itemName      Item name
        //  * @param  {Number} itemPrice     Item price
        //  * @param  {string} itemQty       Item quantity
        //  * @param  {string} itemSku       Item SKU
        //  * @param  {string} itemVariation Item variation / category
        //  * @param  {string} currCode      Currency code
        //  * @param  {string} clientID      uuidV4
        //  * @return {Promise}
        //  */

        /**
         * 
         * @param trnID 
         * @param itemName 
         * @param itemPrice 
         * @param itemQty 
         * @param itemSku 
         * @param itemVariation 
         * @param currCode 
         */
        public Analytics Item(string trnID, string itemName, double? itemPrice = null, string itemQty = null, string itemSku = null, string itemVariation = null, string currCode = null)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ti", trnID);
            dic.Add("in", itemName);



            if (itemPrice != null) dic["ip"] = itemPrice;
            if (itemQty != null) dic["iq"] = itemQty;
            if (itemSku != null) dic["ic"] = itemSku;
            if (itemVariation != null) dic["iv"] = itemVariation;
            if (currCode != null) dic["cu"] = currCode;

            this.Push(this.CreateTracker("item", dic));
            return this;
        }

        /**
         * Send a "timing tracking" request
         * @param  {string} timingCtg     Timing category
         * @param  {string} timingVar     Timing variable
         * @param  {Number} timingTime    Timing time
         * @param  {string} timingLbl     Timing label
         * @param  {Number} dns           DNS load time
         * @param  {Number} pageDownTime  Page download time
         * @param  {Number} redirTime     Redirect time
         * @param  {Number} tcpConnTime   TCP connect time
         * @param  {Number} serverResTime Server response time
         * @param  {string} clientID      uuidV4
         * @return {Promise}
         */
        public Analytics TimingTrk(string timingCtg, string timingVar, int timingTime, string timingLbl, int? dns, int? pageDownTime, int? redirTime, int? tcpConnTime, int? serverResTime)
        {
            var dic = new Dictionary<string, object>();
            dic["utc"] = timingCtg;
            dic["utv"] = timingVar;
            dic["utt"] = timingTime;

            if (timingLbl != null) dic["url"] = timingLbl;
            if (dns != null) dic["dns"] = dns;
            if (pageDownTime != null) dic["pdt"] = pageDownTime;
            if (redirTime != null) dic["rrt"] = redirTime;
            if (tcpConnTime != null) dic["tcp"] = tcpConnTime;
            if (serverResTime != null) dic["srt"] = serverResTime;

            this.Push(this.CreateTracker("timing", dic));
            return this;
        }


        /**
         * create a request params
         * @param hitType hit type
         * @param params param
         */

        /// <summary>
        /// Create a ITracker
        /// </summary>
        /// <param name="hitType"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private ITracker CreateTracker(string hitType, IDictionary<string, object> dic)
        {
            var hit = new Dictionary<string, object> { { "v", this._version }, { "tid", this._tid }, { "cid", this._cid }, { "t", hitType } };
            ITracker tracker = new Tracker(hit);
            tracker.Append(dic);
            return tracker;
        }

        /// <summary>
        ///  Create a ITracker
        /// </summary>
        /// <param name="hitType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private ITracker CreateTracker(string hitType, object obj)
        {
            return CreateTracker(hitType, obj.GetProperties());
        }

        private void Push(ITracker tracker)
        {
            if (tracker == null)
            {
                return;
            }

            this._queue.Enqueue(tracker);
        }

        public Analytics Append(object any)
        {
            if (any == null)
            {
                return this;
            }

            var qstring = any.ToQueryString();
            if (string.IsNullOrEmpty(qstring))
            {
                return this;
            }

            var last = this._queue.LastOrDefault();

            if (last != null)
            {
                last.Append(any);
            }
            else
            {
                this._queue.Enqueue(new Tracker(any));
            }

            return this;
        }

        internal IEnumerable<string> GetBody(IEnumerable<ITracker> p_trackers)
        {
            if (p_trackers == null)
            {
                return Enumerable.Empty<string>();
            }

            return this._queue.Take(20).Select(i => i.GetBody());
        }

        public IEnumerable<string> GetBody()
        {
            return GetBody(_queue);
        }

        /**
         * Send a request to google-analytics
         * @param params param
         */
        public async Task<bool> Send(IEnumerable<ITracker> p_trackers = null)
        {
            var trackers = p_trackers ?? _queue;
            var qstrings = this.GetBody(trackers).ToList();

            var batch = qstrings.Count > 1;
            string url;
            if (this._debug)
            {
                url = $"{BaseUrl}{DebugPath}{CollectPath}";
            }
            else
            {
                var bpath = batch ? BatchPath : CollectPath;
                url = $"{BaseUrl}{bpath}";
            }

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (!string.IsNullOrEmpty(this._proxy))
            {
                httpClientHandler.Proxy = new WebProxy(new Uri(this._proxy), false);
            }

            HttpClient hc = new HttpClient(httpClientHandler);

            if (!string.IsNullOrEmpty(this._userAgent))
            {
                hc.DefaultRequestHeaders.Add("User-Agent", this._userAgent);
            }

            var body = new StringContent(string.Join("\n", qstrings), Encoding.UTF8);
            var response = await hc.PostAsync(url, body);


            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (this._debug)
                {
                    Debug.WriteLine(content);
                    dynamic json = new JDynamic(content);
                    if (json.hitParsingResult[0].valid)
                    {
                        return true;
                    }
                    return false;
                }

                return true;
            }

            if (!string.Equals(response.Content.Headers.ContentType.MediaType, "image/gif",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(content);
            }

            return false;
        }
    }
}
