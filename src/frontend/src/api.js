
const API = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export async function apiGet(path) {
    const r = await fetch(API + path)
    if (!r.ok) return handleErrorResponse(r)
    return r.json()
}
export async function apiPost(path, body) {
    const r = await fetch(API + path, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
    if (!r.ok) return handleErrorResponse(r)
    return r.json()
}
export async function apiPut(path, body) {
    const r = await fetch(API + path, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
    if (!r.ok) return handleErrorResponse(r)
    return r.json()
}
export async function apiDelete(path) {
    const r = await fetch(API + path, { method: 'DELETE' })
    if (!r.ok) return handleErrorResponse(r)
    return r.text()
}

async function handleErrorResponse(r) {
    let errorMessage = 'Erro inesperado'
    try {
        const data = await r.json()
        errorMessage = data?.message || JSON.stringify(data)
    } catch {
        errorMessage = await r.text()
    }
    throw new Error(errorMessage)
}
