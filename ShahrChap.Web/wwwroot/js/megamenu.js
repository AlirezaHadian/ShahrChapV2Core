/* MEGA MENU — باز و بسته شدن (بدون jQuery و بدون Bootstrap)
   با event delegation روی document کار می‌کنه، پس مهم نیست
   اسکریپت توی <head> باشه یا آخر صفحه، یا منو دیرتر ساخته بشه. */
(function () {
  if (window.__mmReady) return;
  window.__mmReady = true;

  var mq = window.matchMedia('(max-width: 790px)');
  var closeTimer, intentTimer;

  function all() { return document.querySelectorAll('[data-mm]'); }

  function setOpen(li, open) {
    li.classList.toggle('is-open', open);
    var t = li.querySelector('.mm-top');
    if (t) t.setAttribute('aria-expanded', open ? 'true' : 'false');
    if (!open) li._byClick = false;
  }
  function closeAll(except) {
    all().forEach(function (li) { if (li !== except) setOpen(li, false); });
  }
  function activate(sub) {
    var li = sub.closest('[data-mm]');
    if (!li) return;
    if (mq.matches) {                          // موبایل: آکاردئون
      var was = sub.classList.contains('is-open');
      li.querySelectorAll('.mm-sub').forEach(function (s) { s.classList.remove('is-open'); });
      if (!was) sub.classList.add('is-open');
    } else {                                   // دسکتاپ: تب
      li.querySelectorAll('.mm-sub').forEach(function (s) { s.classList.remove('is-active'); });
      sub.classList.add('is-active');
    }
  }

  // فاز capture: قبل از هر کلیک‌گیر دیگه‌ی سایت اجرا می‌شه
  // (مثلاً کدی که با کلیک روی لینک‌های منو، سایدبار موبایل رو می‌بنده)
  document.addEventListener('click', function (e) {
    var top = e.target.closest('.mm-top');
    if (top) {
      e.preventDefault();
      e.stopPropagation();
      var li = top.closest('[data-mm]');
      var open = li.classList.contains('is-open');
      if (mq.matches) { closeAll(li); setOpen(li, !open); return; }
      if (open && li._byClick) { setOpen(li, false); }
      else { closeAll(li); setOpen(li, true); li._byClick = true; }
      return;
    }
    var tab = e.target.closest('.mm-tab');
    if (tab) { e.stopPropagation(); activate(tab.parentElement); return; }
    // کلیک روی جاهای خالی پنل (غیر لینک) هم نباید به کد سایت برسه
    // (script.js با هر کلیکی که روی .menu نباشه سایدبار رو می‌بنده)
    if (e.target.closest('.mm-panel') && !e.target.closest('a')) { e.stopPropagation(); return; }
    if (!e.target.closest('[data-mm]')) closeAll();
  }, true);

  document.addEventListener('mouseover', function (e) {
    if (mq.matches) return;
    var li = e.target.closest('[data-mm]');
    if (li) {
      clearTimeout(closeTimer);
      if (!li.classList.contains('is-open')) { closeAll(li); setOpen(li, true); }
    }
    var tab = e.target.closest('.mm-tab');
    if (tab && !(e.relatedTarget && tab.contains(e.relatedTarget))) {
      clearTimeout(intentTimer);             // مکث کوتاه تا موقع حرکت قطری موس تب نپره
      intentTimer = setTimeout(function () { activate(tab.parentElement); }, 90);
    }
  });

  document.addEventListener('mouseout', function (e) {
    if (mq.matches) return;
    var li = e.target.closest('[data-mm]');
    if (li && !(e.relatedTarget && li.contains(e.relatedTarget))) {
      closeTimer = setTimeout(function () { setOpen(li, false); }, 180);
    }
    var tab = e.target.closest('.mm-tab');
    if (tab && !(e.relatedTarget && tab.contains(e.relatedTarget))) clearTimeout(intentTimer);
  });

  document.addEventListener('keydown', function (e) { if (e.key === 'Escape') closeAll(); });
})();
