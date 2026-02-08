<template>
  <VueRecaptcha
    :sitekey="siteKey"
    :load-recaptcha-script="true"
    @verify="handleSuccess"
    @error="handleError"
  ></VueRecaptcha>
</template>

<script>
import { computed, defineComponent } from "vue";
import { VueRecaptcha } from "vue-recaptcha";

export default defineComponent({
  name: "Captcah",
  components: {
    VueRecaptcha,
  },
  setup() {
    const siteKey = computed(() => {
      return "6LcdGLUjAAAAAPqmwHQH5YB1siI6vEgddeqsTOtY";
    });

    const handleError = () => {
      // Do some validation
    };

    const handleSuccess = (response) => {
      // Do some validation
    };

    return {
      handleSuccess,
      handleError,
      siteKey,
    };
  },
});
</script>
