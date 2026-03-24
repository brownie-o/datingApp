import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { AdminService } from '../../../core/services/admin-service';
import { User } from '../../../types/user';

@Component({
  selector: 'app-user-management',
  imports: [],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css',
})
export class UserManagement implements OnInit {
  @ViewChild('rolesModal') rolesModal!: ElementRef<HTMLDialogElement>
  private adminService = inject(AdminService)
  protected users = signal<User[]>([])
  protected availableRoles = ['Member', 'Moderator', 'Admin']
  protected seletcedUser: User | null = null


  ngOnInit(): void {
    this.getUserWithRoles()
  }

  getUserWithRoles() {
    this.adminService.getUserWithRoles().subscribe({
      next: users => this.users.set(users)
    })
  }

  openRolesModal(user: User) {
    this.seletcedUser = user
    this.rolesModal.nativeElement.showModal()
  }

  toggleRole(event: Event, role: string) {
    if (!this.seletcedUser) return

    const isChecked = (event.target as HTMLInputElement).checked
    if (isChecked) {
      this.seletcedUser.roles.push(role)
    } else {
      this.seletcedUser.roles = this.seletcedUser.roles.filter(r => r !== role)
    }
  }

  updateRoles() {
    if (!this.seletcedUser) return

    this.adminService.updateUserRoles(this.seletcedUser.id, this.seletcedUser.roles).subscribe({
      next: updatedRoles => {
        this.users.update(users => users.map(u => {
          if (u.id === this.seletcedUser?.id) u.roles = updatedRoles
          return u
        }));
        this.rolesModal.nativeElement.close()
      },
      error: error => console.log("Failed to update roles", error)
    })
  }
}
