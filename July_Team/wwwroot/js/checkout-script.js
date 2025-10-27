// /js/checkout-script.js (النسخة المعدلة)

document.addEventListener('DOMContentLoaded', () => {
    // --- عناصر الصفحة ---
    const placeOrderBtn = document.getElementById('place-order-btn');
    const checkoutForm = document.getElementById('checkout-form');

    // --- عناصر النافذة المنبثقة ---
    const modal = document.getElementById('order-confirmation-modal');
    const confirmationTotalSpan = document.getElementById('confirmation-total');
    const cancelOrderBtn = document.getElementById('cancel-order');
    const confirmOrderBtn = document.getElementById('confirm-order');

    // --- قراءة المجموع الكلي مباشرة من الصفحة ---
    const grandTotalSpan = document.getElementById('grand-total');

    // --- دالة لإظهار النافذة المنبثقة وتحديث المجموع فيها ---
    function showModal() {
        // تحديث المجموع في النافذة المنبثقة من القيمة المعروضة على الصفحة
        if (grandTotalSpan) {
            confirmationTotalSpan.textContent = grandTotalSpan.textContent;
        }
        modal.style.display = 'block';
        document.body.style.overflow = 'hidden';
    }

    // --- دالة لإخفاء النافذة المنبثقة ---
    function hideModal() {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }

    // --- دالة للتحقق من صحة حقول الإدخال ---
    function validateForm() {
        let isValid = true;
        const requiredFields = ['ShippingFullName', 'ShippingAddress', 'ShippingPhoneNumber'];

        requiredFields.forEach(id => {
            const input = document.getElementById(id);
            if (!input || !input.value.trim()) {
                // يمكنك إضافة منطق لإظهار رسائل الخطأ هنا إذا أردت
                input.classList.add('is-invalid');
                isValid = false;
            } else {
                input.classList.remove('is-invalid');
            }
        });

        // يمكنك إضافة تحقق من صحة رقم الهاتف هنا أيضًا
        return isValid;
    }

    // --- دالة لإرسال النموذج ---
    function submitForm() {
        // ببساطة نقوم بإرسال الفورم
        // الـ Controller سيقوم بالباقي
        checkoutForm.submit();
    }

    // --- ربط الأحداث ---

    // عند الضغط على زر "تأكيد الطلب" الرئيسي
    placeOrderBtn.addEventListener('click', (e) => {
        e.preventDefault();
        if (validateForm()) {
            showModal(); // إذا كانت البيانات صحيحة، أظهر نافذة التأكيد
        } else {
            alert('يرجى ملء جميع الحقول المطلوبة.');
        }
    });

    // عند الضغط على "إلغاء" في النافذة المنبثقة
    cancelOrderBtn.addEventListener('click', hideModal);

    // عند الضغط على "تأكيد الطلب" النهائي في النافذة المنبثقة
    confirmOrderBtn.addEventListener('click', () => {
        // إظهار مؤشر التحميل وتعطيل الزر
        confirmOrderBtn.disabled = true;
        confirmOrderBtn.textContent = 'جاري الإرسال...';

        // إرسال الفورم
        submitForm();
    });

    // إغلاق النافذة عند الضغط خارجها
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            hideModal();
        }
    });
});
