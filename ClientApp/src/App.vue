<template>
  <div>
    <h1>123456789</h1>
    <ul>
      <li v-for="item in products" :key="item.id">
        {{ item.name }}
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  data() {
    return {
      products: []
    }
  },
  mounted() {
    fetch('/api/products')
      .then(res => res.json())
      .then(data => {
        this.products = data
      })
  }
}
</script>