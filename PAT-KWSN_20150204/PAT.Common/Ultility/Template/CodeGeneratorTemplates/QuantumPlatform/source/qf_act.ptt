//////////////////////////////////////////////////////////////////////////////
// Product: QF/C++
// Last Updated for Version: 4.3.00
// Date of the Last Update:  Nov 01, 2011
//
//                    Q u a n t u m     L e a P s
//                    ---------------------------
//                    innovating embedded systems
//
// Copyright (C) 2002-2011 Quantum Leaps, LLC. All rights reserved.
//
// This software may be distributed and modified under the terms of the GNU
// General Public License version 2 (GPL) as published by the Free Software
// Foundation and appearing in the file GPL.TXT included in the packaging of
// this file. Please note that GPL Section 2[b] requires that all works based
// on this software must also be made publicly available under the terms of
// the GPL ("Copyleft").
//
// Alternatively, this software may be distributed and modified under the
// terms of Quantum Leaps commercial licenses, which expressly supersede
// the GPL and are specifically designed for licensees interested in
// retaining the proprietary status of their code.
//
// Contact information:
// Quantum Leaps Web site:  http://www.quantum-leaps.com
// e-mail:                  info@quantum-leaps.com
//////////////////////////////////////////////////////////////////////////////
#include "qf_pkg.h"
#include "qassert.h"

/// \file
/// \ingroup qf
/// \brief QF::active_[], QF::getVersion(), and QF::add_()/QF::remove_()
/// implementation.

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

Q_DEFINE_THIS_MODULE(qf_act)

// public objects ------------------------------------------------------------
QActive *QF::active_[QF_MAX_ACTIVE + 1];        // to be used by QF ports only
uint8_t QF_intLockNest_;                       // interrupt-lock nesting level

//............................................................................
//lint -e970 -e971               ignore MISRA rules 13 and 14 in this function
const char Q_ROM * Q_ROM_VAR QF::getVersion(void) {
    static char const Q_ROM Q_ROM_VAR version[] = {
        (char)(((QP_VERSION >> 12U) & 0xFU) + (uint8_t)'0'),
        '.',
        (char)(((QP_VERSION >>  8U) & 0xFU) + (uint8_t)'0'),
        '.',
        (char)(((QP_VERSION >>  4U) & 0xFU) + (uint8_t)'0'),
        (char)((QP_VERSION          & 0xFU) + (uint8_t)'0'),
        '\0'
    };
    return version;
}
//............................................................................
void QF::add_(QActive *a) {
    uint8_t p = a->m_prio;

    Q_REQUIRE(((uint8_t)0 < p) && (p <= (uint8_t)QF_MAX_ACTIVE)
              && (active_[p] == (QActive *)0));

    QF_CRIT_STAT_
    QF_CRIT_ENTRY_();

    active_[p] = a;            // registger the active object at this priority

    QS_BEGIN_NOCRIT_(QS_QF_ACTIVE_ADD, QS::aoObj_, a)
        QS_TIME_();                                               // timestamp
        QS_OBJ_(a);                                       // the active object
        QS_U8_(p);                        // the priority of the active object
    QS_END_NOCRIT_()

    QF_CRIT_EXIT_();
}
//............................................................................
void QF::remove_(QActive const *a) {
    uint8_t p = a->m_prio;

    Q_REQUIRE(((uint8_t)0 < p) && (p <= (uint8_t)QF_MAX_ACTIVE)
              && (active_[p] == a));

    QF_CRIT_STAT_
    QF_CRIT_ENTRY_();

    active_[p] = (QActive *)0;                   // free-up the priority level

    QS_BEGIN_NOCRIT_(QS_QF_ACTIVE_REMOVE, QS::aoObj_, a)
        QS_TIME_();                                               // timestamp
        QS_OBJ_(a);                                       // the active object
        QS_U8_(p);                        // the priority of the active object
    QS_END_NOCRIT_()

    QF_CRIT_EXIT_();
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif

