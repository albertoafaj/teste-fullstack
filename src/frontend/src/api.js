
const API = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export async function apiGet(path) {
    try {
        const r = await fetch(API + path)
        if (!r.ok) return handleErrorResponse(r)
        return r.json()
    } catch (err) {
        throw new Error(`Falha de conexão com a API. Detalhes: ${err.message}`)
    }
}

export async function apiPost(path, body) {
    try {
        const r = await fetch(API + path, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
        if (!r.ok) return handleErrorResponse(r)
        return r.json()
    } catch (err) {
        throw new Error(`Falha de conexão com a API. Detalhes: ${err.message}`)
    }
}

export async function apiPut(path, body) {
    try {
        const r = await fetch(API + path, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
        if (!r.ok) return handleErrorResponse(r)
        return r.json()
    } catch (err) {
        throw new Error(`Falha de conexão com a API. Detalhes: ${err.message}`)
    }
}

export async function apiDelete(path) {
    try {
        const r = await fetch(API + path, { method: 'DELETE' })
        if (!r.ok) return handleErrorResponse(r)
        return r.text()
    } catch (err) {
        throw new Error(`Falha de conexão com a API. Detalhes: ${err.message}`)
    }
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
