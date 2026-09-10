/** @type {import('tailwindcss').Config} */
export default {
  content: ["./**/*.razor", "./**/*.html"],
  theme: {
    extend: {
      colors: {
        base: { 950: '#09090b', 900: '#18181b', 800: '#27272a', 700: '#3f3f46' },
      },
      animation: {
        'pulse-slow': 'pulse 3s cubic-bezier(0.4, 0, 0.6, 1) infinite',
      },
    },
  },
  plugins: [],
}