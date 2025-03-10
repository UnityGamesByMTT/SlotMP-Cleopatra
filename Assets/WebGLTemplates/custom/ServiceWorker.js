const cacheName = {{{JSON.stringify(COMPANY_NAME + "-" + PRODUCT_NAME + "-" + PRODUCT_VERSION )}}};
const contentToCache = [
    "Build/{{{ LOADER_FILENAME }}}",
    "Build/{{{ FRAMEWORK_FILENAME }}}",
    "Build/{{{ DATA_FILENAME }}}",
    "Build/{{{ CODE_FILENAME }}}",
];

self.addEventListener("install", function (e) {
  console.log("[Service Worker] Install");

  e.waitUntil(
    caches.keys().then((cacheNames) => {
      return Promise.all(
        cacheNames
          .filter((name) => {
            return name !== cacheName;
          })
          .map((name) => {
            console.log("[Service Worker] Deleting old cache:", name);
            return caches.delete(name);
          })
      );
    })
  );

  e.waitUntil(
    (async function () {
      const cache = await caches.open(cacheName);
      console.log("[Service Worker] Caching all: app shell and content");
      await cache.addAll(contentToCache);
    })()
  );
});

self.addEventListener('fetch', function (e) {
  console.log("Called")
  e.respondWith((async function () {
     try {
         const response = await caches.match(e.request);
         console.log([Service Worker] Fetching resource: ${e.request.url});
         if (response) return response;

         const fetchResponse = await fetch(e.request);
         const cache = await caches.open(cacheName);
         console.log([Service Worker] Caching new resource: ${e.request.url});
         await cache.put(e.request, fetchResponse.clone());
         return fetchResponse;
     } catch (error) {
         console.error([Service Worker] Error fetching resource: ${e.request.url}, error);
         throw error;
     }
 })());
});
