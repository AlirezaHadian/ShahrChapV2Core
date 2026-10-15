/* SEARCH RESULTS — هایلایت کلمه‌ی جستجو + نمای شبکه/لیست */
(function () {
  var root = document.querySelector('.sr-scope');
  if (!root) return;

  function highlight() {
    var q = (root.getAttribute('data-q') || '').trim();
    if (!q) return;
    var words = q.split(/\s+/).filter(Boolean).map(function (w) {
      return w.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    });
    var re = new RegExp('(' + words.join('|') + ')', 'gi');
    root.querySelectorAll('.sr-name').forEach(function (el) {
      var t = el.textContent.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
      el.innerHTML = t.replace(re, '<mark>$1</mark>');
    });
  }
  window.srHighlight = highlight;
  highlight();

  var grid = function () { return root.querySelector('.sr-grid'); };
  function setView(v) {
    var g = grid(); if (g) g.classList.toggle('is-list', v === 'list');
    root.querySelectorAll('.sr-view button').forEach(function (b) { b.classList.toggle('on', b.dataset.v === v); });
    try { localStorage.setItem('srView', v); } catch (e) {}
  }
  root.addEventListener('click', function (e) {
    var b = e.target.closest('.sr-view button'); if (b) setView(b.dataset.v);
  });
  var saved; try { saved = localStorage.getItem('srView'); } catch (e) {}
  if (saved === 'list') setView('list');
})();
