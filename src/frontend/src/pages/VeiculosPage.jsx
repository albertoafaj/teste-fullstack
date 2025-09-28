import React, { useEffect, useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiGet, apiPost, apiPut, apiDelete } from '../api'

export default function VeiculosPage() {
    const qc = useQueryClient()
    const [clienteId, setClienteId] = useState('')
    const clientes = useQuery({ queryKey: ['clientes-mini'], queryFn: () => apiGet('/api/clientes?pagina=1&tamanho=100') })
    const veiculos = useQuery({ queryKey: ['veiculos', clienteId], queryFn: () => apiGet(`/api/veiculos${clienteId ? `?clienteId=${clienteId}` : ''}`) })
    const [form, setForm] = useState({ placa: '', modelo: '', ano: '', clienteId: '' })
    const [editId, setEditId] = useState(null)
    const [errorMessage, setErrorMessage] = useState('')
    const [status, setStatus] = useState('Todos')

    const create = useMutation({
        mutationFn: (data) => apiPost('/api/veiculos', data),
        onSuccess: () => {
            qc.invalidateQueries({ queryKey: ['veiculos'] })
            setErrorMessage('')
        },
        onError: (error) => {
            setErrorMessage(error.message)
        }
    })

    const update = useMutation({
        mutationFn: (data) => apiPut(`/api/veiculos/${editId}`, data),
        onSuccess: () => {
            qc.invalidateQueries({ queryKey: ['veiculos'] })
            setErrorMessage('')
        },
        onError: (error) => {
            setErrorMessage(error.message)
        }
    })

    const remover = useMutation({
        mutationFn: (id) => apiDelete(`/api/veiculos/${id}`),
        onSuccess: () => qc.invalidateQueries({ queryKey: ['veiculos'] })
    })

    useEffect(() => {
        if (clientes.data?.itens?.length && !clienteId) {
            setClienteId(clientes.data.itens[0].id)
            setForm(f => ({ ...f, clienteId: clientes.data.itens[0].id }))
        }
        if (errorMessage) {
            setErrorMessage('')
        }
    }, [clientes.data])

    return (
        <div>
            <h2>Veículos</h2>

            <div className="section">
                <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
                    <label>Cliente: </label>
                    <select value={clienteId} onChange={e => { setClienteId(e.target.value); setForm(f => ({ ...f, clienteId: e.target.value })) }}>
                        {clientes.data?.itens?.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
                    </select>
                    <label>Status: </label>
                    <select
                        value={status}
                        onChange={e => {
                            setStatus(e.target.value);
                        }}>
                        <option key="Todos" value="Todos">Todos</option>
                        <option key="Ativo" value="Ativo">Vigentes</option>
                        <option key="Inativo" value="Inativo">Inativos</option>
                    </select>
                </div>
            </div>

            <h3>Novo veículo</h3>
            <div className="section">
                <div className="grid grid-4">
                    <input placeholder="Placa" value={form.placa} onChange={e => setForm({ ...form, placa: e.target.value })} />
                    <input placeholder="Modelo" value={form.modelo} onChange={e => setForm({ ...form, modelo: e.target.value })} />
                    <input placeholder="Ano" value={form.ano} onChange={e => setForm({ ...form, ano: e.target.value })} />
                    <button onClick={() => create.mutate({
                        placa: form.placa, modelo: form.modelo, ano: form.ano ? Number(form.ano) : null, clienteId: form.clienteId || clienteId
                    })}>Salvar</button>
                </div>
            </div>

            <h3 style={{ marginTop: 16 }}>Lista</h3>
            <div className="section">
                {veiculos.isLoading ? <p>Carregando...</p> : (
                    <table>
                        <thead><tr><th>Placa</th><th>Modelo</th><th>Ano</th><th>ClienteId</th><th>Ações</th></tr></thead>
                        <tbody>
                            {

                                veiculos.data
                                    ?.filter(v => {
                                        if (status === 'Todos') return true
                                        if (status === 'Ativo') return v.dataVigencia == null
                                        if (status === 'Inativo') return v.dataVigencia != null
                                        return true
                                    })
                                    .map(v => (
                                        <tr key={v.id}>
                                            <td>{v.placa}</td>
                                            <td>{v.modelo}</td>
                                            <td>{v.ano ?? '-'}</td>
                                            <td>{v.clienteId}</td>
                                            <td style={{ display: 'flex', gap: 8 }}>
                                                <button className="btn-ghost" onClick={() => {
                                                    const novoModelo = prompt('Novo modelo', v.modelo || '')
                                                    const novoAno = prompt('Novo ano', v.ano || '')
                                                    if (novoModelo == null && novoAno == null) return
                                                    setEditId(v.id)
                                                    update.mutate({ placa: v.placa, modelo: novoModelo, ano: novoAno, clienteId: v.clienteId })
                                                }}>Editar Dados</button>

                                                <button className="btn-ghost" onClick={() => remover.mutate(v.id)}>Excluir</button>

                                                {v.dataVigencia == null ?
                                                    <button className="btn-ghost" onClick={() => {
                                                        const novoCliente = prompt('Novo Cliente', v.clienteId || '')
                                                        if (novoCliente == null) return
                                                        setEditId(v.id)
                                                        update.mutate({ placa: v.placa, modelo: v.modelo, ano: v.ano, clienteId: novoCliente })
                                                    }}>Trocar Cliente</button> :
                                                    < div style={{ padding: '9px 14px' }}>Veículo inativo</div>
                                                }
                                            </td>
                                        </tr>
                                    ))}
                        </tbody>
                    </table>
                )}
                {errorMessage && (
                    <div style={{ color: 'red', marginBottom: 8 }}>
                        {errorMessage}

                    </div>
                )}

                {clientes.isError && (
                    <div style={{ color: 'red', marginBottom: 8 }}>
                        {clientes.error.message}
                    </div>
                )}

                {veiculos.isError && (
                    <div style={{ color: 'red', marginBottom: 8 }}>
                        {veiculos.error.message}
                    </div>
                )}

            </div>
        </div>
    )
}
