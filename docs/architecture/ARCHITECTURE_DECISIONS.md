# Architecture Decision Records

## ADR-001: Dual Communication Model

**Status:** Accepted

**Decision:**
- Sync: gRPC for real-time queries (chat, semantic search)
- Async: Redis Streams for background processing (ingestion, embeddings)

**Rationale:**
Query needs immediate response, ingestion can be deferred.
EOF

cat > docs/architecture/SYSTEM_BOUNDARIES.md << 'EOF'
# System Boundaries

## Core Backend (.NET)
- Source of truth: Users, Documents metadata, Audit logs
- Responsibilities: Auth, orchestration, business rules

## AI Layer (Python)
- Derived data: Embeddings, Vector index, LLM responses
- Responsibilities: Intelligence, retrieval, reasoning