/**
 * Shared inline form validation (Bootstrap is-invalid + invalid-feedback), aligned with User wizard pattern.
 */
(function (w) {
    'use strict';

    function byId(id) {
        return id ? document.getElementById(id) : null;
    }

    w.InlineFormValidation = {
        setFieldError: function (inputId, errorElementId, message) {
            var input = byId(inputId);
            var err = byId(errorElementId);
            if (input) input.classList.add('is-invalid');
            if (err) {
                err.textContent = message || 'Invalid value.';
                err.classList.add('d-block');
            }
        },

        clearFieldError: function (inputId, errorElementId) {
            var input = byId(inputId);
            var err = byId(errorElementId);
            if (input) input.classList.remove('is-invalid');
            if (err) {
                err.textContent = '';
                err.classList.remove('d-block');
            }
        },

        /** fieldMap: { inputId: errorElementId, ... } */
        clearMap: function (fieldMap) {
            var self = this;
            if (!fieldMap) return;
            Object.keys(fieldMap).forEach(function (inputId) {
                self.clearFieldError(inputId, fieldMap[inputId]);
            });
        },

        bindAutoClear: function (fieldMap) {
            var self = this;
            if (!fieldMap) return;
            Object.keys(fieldMap).forEach(function (inputId) {
                var el = byId(inputId);
                if (!el) return;
                var errId = fieldMap[inputId];
                var handler = function () {
                    self.clearFieldError(inputId, errId);
                };
                el.addEventListener('input', handler);
                el.addEventListener('change', handler);
            });
        },

        setNotice: function (elementId, message) {
            var el = byId(elementId);
            if (!el) return;
            el.textContent = message || '';
            el.classList.toggle('d-none', !message);
        },

        clearNotice: function (elementId) {
            this.setNotice(elementId, '');
        },

        isNonEmpty: function (v) {
            return v != null && String(v).trim().length > 0;
        },

        isValidEmail: function (email) {
            if (!email || !String(email).trim()) return true;
            return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(email).trim());
        }
    };
})(window);
