# ============================================
# Nexo Intelligence Platform - Makefile
# ============================================

.PHONY: help setup up down logs test lint clean

# Colors for output
GREEN := \033[0;32m
RED := \033[0;31m
NC := \033[0m # No Color

help:
	@echo "$(GREEN)Nexo Intelligence Platform Commands$(NC)"
	@echo ""
	@echo "  $(GREEN)make setup$(NC)     - Initialize development environment"
	@echo "  $(GREEN)make up$(NC)        - Start all services (Docker Compose)"
	@echo "  $(GREEN)make down$(NC)      - Stop all services"
	@echo "  $(GREEN)make logs$(NC)      - View all logs"
	@echo "  $(GREEN)make test$(NC)      - Run all tests"
	@echo "  $(GREEN)make lint$(NC)      - Run linters"
	@echo "  $(GREEN)make clean$(NC)     - Clean build artifacts"

setup:
	@echo "$(GREEN)Setting up development environment...$(NC)"
	cp .env.example .env
	docker-compose -f infra/docker/docker-compose.yml up -d postgres redis
	cd src/backend && dotnet restore
	cd src/ai-layer && python -m venv .venv && source .venv/bin/activate && pip install -r requirements.txt
	@echo "$(GREEN)Setup complete! Run 'make up' to start all services.$(NC)"

up:
	@echo "$(GREEN)Starting all services...$(NC)"
	docker-compose -f infra/docker/docker-compose.yml up -d
	@echo "$(GREEN)Services started. Run 'make logs' to see output.$(NC)"

down:
	@echo "$(GREEN)Stopping all services...$(NC)"
	docker-compose -f infra/docker/docker-compose.yml down

logs:
	docker-compose -f infra/docker/docker-compose.yml logs -f

test:
	@echo "$(GREEN)Running backend tests...$(NC)"
	cd src/backend && dotnet test
	@echo "$(GREEN)Running AI layer tests...$(NC)"
	cd src/ai-layer && source .venv/bin/activate && pytest tests/ -v

lint:
	@echo "$(GREEN)Linting backend...$(NC)"
	cd src/backend && dotnet format --verify-no-changes
	@echo "$(GREEN)Linting AI layer...$(NC)"
	cd src/ai-layer && source .venv/bin/activate && ruff check .
	@echo "$(GREEN)Type checking AI layer...$(NC)"
	cd src/ai-layer && source .venv/bin/activate && mypy app/

clean:
	@echo "$(GREEN)Cleaning build artifacts...$(NC)"
	cd src/backend && dotnet clean
	rm -rf src/ai-layer/.pytest_cache src/ai-layer/htmlcov
	docker system prune -f