/* SEARCH SUGGEST — اتوکامپلیت جستجو
   روی هر <input data-ac="/Search/Suggest"> فعال می‌شه (هدر و صفحه‌ی نتایج).
   قرارداد JSON سرور:
   GET <data-ac>?q=...  →  {
     "total": 12,
     "groups":   [ { "id":3, "title":"کارت ویزیت", "count":8, "url":"/Search?q=...&groupId=3" } ],
     "products": [ { "id":5, "title":"...", "group":"کارت ویزیت", "image":"/product/thumb/x.jpg", "price":30000, "url":"/Product/ShowProduct/5" } ]
   }  */
(function() {
    'use strict';
    if (window.__acReady) return;
    window.__acReady = true;

    var MIN = 2, DELAY = 220, SKELETON_DELAY = 120, MAX_RECENT = 6, KEY = 'sc_recent_searches', uid = 0;

    function esc(s) {
        return String(s == null ? '' : s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }
    function clean(s) { return (s || '').replace(/\s+/g, ' ').trim(); }
    function safeUrl(u) { return (typeof u === 'string' && /^\/(?!\/)/.test(u)) ? u : '#'; }
    function fmt(n) { return Number(n).toLocaleString('en-US'); }

    // هایلایت روی متن خام (نه روی HTML escape شده) تا ساختار HTML خراب نشه
    function hl(text, q) {
        text = String(text == null ? '' : text);
        var words = clean(q).split(' ').filter(Boolean).map(function(w) { return w.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); });
        if (!words.length) return esc(text);
        var re = new RegExp('(' + words.join('|') + ')', 'gi'), out = '', last = 0, m;
        while ((m = re.exec(text))) {
            out += esc(text.slice(last, m.index)) + '<mark>' + esc(m[0]) + '</mark>';
            last = m.index + m[0].length;
            if (m[0].length === 0) re.lastIndex++;
        }
        return out + esc(text.slice(last));
    }

    function getRecent() { try { return JSON.parse(localStorage.getItem(KEY)) || []; } catch (e) { return []; } }
    function setRecent(a) { try { localStorage.setItem(KEY, JSON.stringify(a.slice(0, MAX_RECENT))); } catch (e) { } }
    function pushRecent(q) {
        q = clean(q); if (q.length < MIN) return;
        var a = getRecent().filter(function(x) { return x !== q; });
        a.unshift(q); setRecent(a);
    }

    function init(input) {
        if (input.__ac) return;
        input.__ac = true;

        var url = input.getAttribute('data-ac');
        var form = input.form;
        var allUrl = (form && form.getAttribute('action')) || input.getAttribute('data-ac-all') || '/Search';
        var host = input.parentElement;

        var panel = document.createElement('div');
        panel.className = 'ac-panel';
        panel.id = 'ac-panel-' + (++uid);
        panel.setAttribute('role', 'listbox');
        panel.hidden = true;
        host.appendChild(panel);

        input.setAttribute('autocomplete', 'off');
        input.setAttribute('role', 'combobox');
        input.setAttribute('aria-autocomplete', 'list');
        input.setAttribute('aria-expanded', 'false');
        input.setAttribute('aria-controls', panel.id);

        var opts = [], active = -1, timer, skelTimer, ctrl, cache = {};

        function allLink(q) { return allUrl + (allUrl.indexOf('?') < 0 ? '?' : '&') + 'q=' + encodeURIComponent(q); }

        function setActive(i) {
            if (opts[active]) { opts[active].classList.remove('is-active'); opts[active].setAttribute('aria-selected', 'false'); }
            active = i;
            if (opts[i]) {
                opts[i].classList.add('is-active');
                opts[i].setAttribute('aria-selected', 'true');
                input.setAttribute('aria-activedescendant', opts[i].id);
                if (opts[i].scrollIntoView) opts[i].scrollIntoView({ block: 'nearest' });
            } else {
                input.removeAttribute('aria-activedescendant');
            }
        }
        function open() { if (panel.hidden) { panel.hidden = false; input.setAttribute('aria-expanded', 'true'); } }
        function close() { panel.hidden = true; input.setAttribute('aria-expanded', 'false'); setActive(-1); }
        function paint(html) {
            panel.innerHTML = html;
            opts = Array.prototype.slice.call(panel.querySelectorAll('[role=option]'));
            opts.forEach(function(o, i) { o.id = panel.id + '-o' + i; o.setAttribute('aria-selected', 'false'); });
            active = -1;
            input.removeAttribute('aria-activedescendant');
            open();
        }

        /* ---------- نماها ---------- */
        function viewRecent() {
            var r = getRecent();
            if (!r.length) {
                paint('<div class="ac-hint"><span class="uil uil-search-alt"></span>برای دیدن پیشنهادها حداقل ' + MIN + ' حرف بنویس</div>');
                return;
            }
            paint('<div class="ac-lbl"><span>جستجوهای اخیر</span><button type="button" class="ac-clear" data-ac-clear>پاک کردن</button></div>' +
                r.map(function(t) {
                    return '<div class="ac-item ac-recent" role="option" data-q="' + esc(t) + '">' +
                        '<span class="uil uil-history ac-ico"></span><span class="ac-tt">' + esc(t) + '</span>' +
                        '<button type="button" class="ac-x" data-ac-del="' + esc(t) + '" aria-label="حذف"><span class="uil uil-times"></span></button></div>';
                }).join(''));
        }
        function viewLoading() {
            var row = '<div class="ac-sk"><span class="ac-sk-th"></span><span class="ac-sk-tx"><span></span><span></span></span></div>';
            paint('<div class="ac-lbl"><span>در حال جستجو...</span></div>' + row + row + row);
        }
        function viewMessage(q, icon, title, sub) {
            paint('<div class="ac-empty"><span class="uil ' + icon + '"></span><b>' + title + '</b><small>' + sub + '</small></div>' +
                '<a class="ac-all" role="option" href="' + esc(allLink(q)) + '"><span>جستجوی کامل «<b>' + esc(q) + '</b>»</span><span class="uil uil-arrow-left"></span></a>');
        }
        function productRow(p, q) {
            return '<a class="ac-item" role="option" href="' + esc(safeUrl(p.url || ('/Product/ShowProduct/' + p.id))) + '">' +
                '<span class="ac-th">' + (p.image ? '<img src="' + esc(p.image) + '" alt="" loading="lazy">' : '') + '</span>' +
                '<span class="ac-tx"><span class="ac-tt">' + hl(p.title, q) + '</span><span class="ac-sub">' + esc(p.group || '') + '</span></span>' +
                '<span class="ac-pr">' + (p.price ? '<small>از</small> ' + fmt(p.price) + ' <small>تومان</small>' : '<small>استعلام قیمت</small>') + '</span></a>';
        }
        function viewResults(q, d) {
            var gs = d.groups || [], ps = d.products || [], total = d.total != null ? d.total : ps.length;
            if (!gs.length && !ps.length) {
                return viewMessage(q, 'uil-search-alt', 'نتیجه‌ای برای «' + esc(q) + '» پیدا نشد', 'املای کلمه را بررسی کن یا کلمه‌ی کلی‌تری بنویس');
            }
            var h = '';
            if (gs.length) {
                h += '<div class="ac-lbl"><span>گروه‌ها</span></div><div class="ac-chips">' + gs.map(function(g) {
                    return '<a class="ac-chip" role="option" href="' + esc(safeUrl(g.url)) + '"><span class="uil uil-folder"></span>' + hl(g.title, q) +
                        (g.count != null ? '<em>' + esc(g.count) + '</em>' : '') + '</a>';
                }).join('') + '</div>';
            }
            if (ps.length) h += '<div class="ac-lbl"><span>محصولات</span></div>' + ps.map(function(p) { return productRow(p, q); }).join('');
            h += '<a class="ac-all" role="option" href="' + esc(allLink(q)) + '"><span>مشاهده همه ' + esc(total) + ' نتیجه برای «<b>' + esc(q) + '</b>»</span><span class="uil uil-arrow-left"></span></a>';
            paint(h);
        }

        /* ---------- دریافت پیشنهادها ---------- */
        function run(q) {
            if (cache[q]) { viewResults(q, cache[q]); return; }
            if (ctrl) ctrl.abort();
            ctrl = ('AbortController' in window) ? new AbortController() : null;
            clearTimeout(skelTimer);
            skelTimer = setTimeout(viewLoading, SKELETON_DELAY);      // اسکلت فقط اگه جواب دیر برسه
            fetch(url + (url.indexOf('?') < 0 ? '?' : '&') + 'q=' + encodeURIComponent(q), {
                headers: { 'Accept': 'application/json' }, credentials: 'same-origin', signal: ctrl ? ctrl.signal : undefined
            })
                .then(function(r) { if (!r.ok) throw new Error(r.status); return r.json(); })
                .then(function(d) {
                    clearTimeout(skelTimer);
                    cache[q] = d;
                    if (clean(input.value) === q) viewResults(q, d);       // جواب کهنه رو نادیده بگیر
                })
                .catch(function(err) {
                    clearTimeout(skelTimer);
                    if (err && err.name === 'AbortError') return;
                    if (clean(input.value) === q) viewMessage(q, 'uil-wifi-slash', 'ارتباط برقرار نشد', 'اتصال اینترنت را بررسی کن و دوباره تلاش کن');
                });
        }

        function onFocus() {
            var q = clean(input.value);
            if (q.length < MIN) viewRecent(); else run(q);
        }
        function choose(el) {
            if (!el) return;
            if (el.tagName === 'A') { pushRecent(input.value); window.location.href = el.getAttribute('href'); return; }
            if (el.hasAttribute('data-q')) { input.value = el.getAttribute('data-q'); submit(); }
        }
        function submit() {
            var q = clean(input.value); if (!q) return;
            pushRecent(q);
            if (form) { if (form.requestSubmit) form.requestSubmit(); else form.submit(); }
            else window.location.href = allLink(q);
        }

        /* ---------- رویدادها ---------- */
        input.addEventListener('input', function() {
            var q = clean(input.value);
            clearTimeout(timer); clearTimeout(skelTimer);
            if (ctrl) ctrl.abort();
            if (q.length < MIN) { viewRecent(); return; }
            timer = setTimeout(function() { run(q); }, DELAY);
        });
        input.addEventListener('focus', onFocus);
        input.addEventListener('click', function() { if (panel.hidden) onFocus(); });

        input.addEventListener('keydown', function(e) {
            if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                if (panel.hidden) { onFocus(); e.preventDefault(); return; }
                if (!opts.length) return;
                e.preventDefault();
                var n = e.key === 'ArrowDown' ? active + 1 : active - 1;
                if (n >= opts.length) n = -1;
                if (n < -1) n = opts.length - 1;
                setActive(n);
            } else if (e.key === 'Enter') {
                if (!panel.hidden && active >= 0) { e.preventDefault(); choose(opts[active]); }
                else if (!form) { e.preventDefault(); submit(); }
            } else if (e.key === 'Escape') {
                if (!panel.hidden) { e.preventDefault(); close(); }
            } else if (e.key === 'Tab') {
                close();
            }
        });

        panel.addEventListener('click', function(e) {
            var del = e.target.closest('[data-ac-del]');
            if (del) { e.preventDefault(); setRecent(getRecent().filter(function(x) { return x !== del.getAttribute('data-ac-del'); })); viewRecent(); return; }
            if (e.target.closest('[data-ac-clear]')) { setRecent([]); viewRecent(); return; }
            var rec = e.target.closest('[data-q]');
            if (rec) { choose(rec); return; }
            if (e.target.closest('a.ac-item, a.ac-chip, a.ac-all')) pushRecent(input.value);
        });
        panel.addEventListener('mouseover', function(e) {
            var o = e.target.closest('[role=option]'); if (!o) return;
            var i = opts.indexOf(o); if (i > -1 && i !== active) setActive(i);
        });
        panel.addEventListener('error', function(e) {          // عکسی که لود نشد مخفی شه
            if (e.target && e.target.tagName === 'IMG') e.target.style.visibility = 'hidden';
        }, true);

        if (form) form.addEventListener('submit', function(e) {
            var q = clean(input.value);
            if (!q) { e.preventDefault(); return; }
            pushRecent(q); close();
        });
        document.addEventListener('click', function(e) { if (!host.contains(e.target)) close(); });
    }

    function scan() { document.querySelectorAll('input[data-ac]').forEach(init); }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', scan); else scan();
})();