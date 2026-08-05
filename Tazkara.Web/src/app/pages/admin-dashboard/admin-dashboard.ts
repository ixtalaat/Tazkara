import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin.service';
import { AdminOverview } from '../../models/types';
import Swal from 'sweetalert2';

@Component({ selector: 'app-admin-dashboard', imports: [NgIf, NgFor, FormsModule, RouterLink, CurrencyPipe, DatePipe], templateUrl: './admin-dashboard.html', styleUrl: './admin-dashboard.css' })
export class AdminDashboardComponent implements OnInit {
  private service = inject(AdminService);
  data = signal<AdminOverview | null>(null); loading = signal(true); error = signal(''); activeTab = signal<'review' | 'users' | 'categories'>('review'); actionId = signal<string | null>(null); newCategoryName = ''; addingCategory = false;
  ngOnInit(): void { this.load(); }
  load(): void { this.loading.set(true); this.service.overview().subscribe({ next: r => { this.loading.set(false); r.success && r.data ? this.data.set(r.data) : this.error.set(r.message || 'Unable to load admin overview.'); }, error: e => { this.loading.set(false); this.error.set(e.error?.message || 'Unable to load admin overview.'); } }); }
  statusClass(status: string): string { return `badge-${status.toLowerCase()}`; }
  review(id: string, action: 'publish' | 'reject'): void { this.actionId.set(id); const request = action === 'publish' ? this.service.publishEvent(id) : this.service.rejectEvent(id); request.subscribe({ next: r => { this.actionId.set(null); r.success ? this.load() : this.error.set(r.message); }, error: e => { this.actionId.set(null); this.error.set(e.error?.message || 'Action failed.'); } }); }
  addCategory(): void {
    const name = this.newCategoryName.trim();
    if (!name || this.addingCategory) return;
    this.addingCategory = true; this.error.set('');
    this.service.createCategory(name).subscribe({
      next: async r => { this.addingCategory = false; if (r.success) { this.newCategoryName = ''; await Swal.fire({ icon: 'success', title: 'Category added', text: `${name} is now available for events.`, timer: 1800, showConfirmButton: false }); this.load(); } else { this.showError(r.message || 'Unable to create category.'); } },
      error: e => { this.addingCategory = false; this.showError(e.error?.message || 'Unable to create category.'); }
    });
  }
  async deleteCategory(id: string): Promise<void> {
    const category = this.data()?.categories.find(item => item.id === id);
    const confirmation = await Swal.fire({ icon: 'warning', title: 'Delete category?', text: `This will remove “${category?.name ?? 'this category'}” permanently.`, showCancelButton: true, confirmButtonText: 'Yes, delete it', cancelButtonText: 'Cancel', confirmButtonColor: '#f43f5e', background: '#111628', color: '#f3f4f6' });
    if (!confirmation.isConfirmed) return;
    this.service.deleteCategory(id).subscribe({ next: async r => { if (r.success) { await Swal.fire({ icon: 'success', title: 'Category deleted', timer: 1600, showConfirmButton: false }); this.load(); } else this.showError(r.message); }, error: e => this.showError(e.error?.message || 'Unable to delete category.') });
  }
  private showError(message: string): void { this.error.set(message); Swal.fire({ icon: 'error', title: 'Action failed', text: message, background: '#111628', color: '#f3f4f6', confirmButtonColor: '#6366f1' }); }
}
